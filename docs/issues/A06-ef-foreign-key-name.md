# [A06 / EF Core] 建表遗漏外键名，后续 DROP 返回成功但约束未移除

## 完整独立 Repro.csproj

固定 .NET 10、EF Core 10.0.12 与 SonnetDB 4.0.0；没有跨仓库 ProjectReference 或引擎源码引用。在各自空目录保存 `Repro.csproj` 和对应的 `Program.cs`。

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SonnetDB.EntityFrameworkCore" Version="4.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.12" />
  </ItemGroup>
</Project>
```


### 问题与影响

官方 ABP BookStore 为外键提供固定名称，例如书籍到作者的外键。公开 SonnetDB Provider 的 CREATE TABLE SQL 未保留 `HasConstraintName`：引擎最终使用自动名称 `fk_A06Books_1`。随后 EF `DropForeignKeyOperation` 使用模型中的名字 `FK_A06_Book_Author`；执行 DROP 没有报错，但约束未被移除，后续允许孤儿数据的目标模型仍被旧外键拒绝。

这会影响后续调整/替换关系的标准迁移，产生操作返回成功与真实 schema 不一致的问题。初次建表和原外键约束本身可以工作，不能描述为“SonnetDB 完全不支持外键”。

### 已验证结果

本机 PowerShell 7.6.6、.NET SDK 10.0.401，包版本如上，嵌入式专属数据库。SonnetDB 实际退出 1：

```sql
CREATE TABLE "A06Books" (...,
    PRIMARY KEY ("Id"),
    FOREIGN KEY ("AuthorId") REFERENCES "A06Authors" ("Id") ON DELETE CASCADE
);

ALTER TABLE "A06Books" DROP CONSTRAINT "FK_A06_Book_Author";
```

DROP 的执行没有抛异常，失败发生于随后 `SaveChangesAsync` 插入孤儿书籍：

```text
Microsoft.EntityFrameworkCore.DbUpdateException
SonnetDB.Tables.TableConstraintException:
外键 'fk_A06Books_1' 冲突：table 'A06Books' 引用了不存在的 'A06Authors' 主键。
```

SQLite 对照采用无外键的正确目标模型，由 Provider 重建表移除约束，之后孤儿插入成功。目标模型与来源模型使用独立 DbContext 类型，避免模型缓存把同一旧外键错误带入对照。

本地完整日志：`artifacts/checks/a06-gap-20261011-refined-sonnetdb-named-foreign-key.stdout.log` 和对应 `.stderr.log`。数据库由该 probe 创建并完成所有权核验后清理，失败日志保留。

### 独立 Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

if (args.Length != 1 || args[0] is not ("Sqlite" or "SonnetDB"))
{
    Console.Error.WriteLine("Usage: dotnet run -- Sqlite|SonnetDB");
    return 2;
}

var owner = Guid.NewGuid().ToString("N");
var parent = Path.GetFullPath(Environment.CurrentDirectory);
var directory = Path.Combine(parent, "a06-fk-" + owner);
Directory.CreateDirectory(directory);
await File.WriteAllTextAsync(Path.Combine(directory, ".owner"), owner);
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(25));
var result = 1;
try
{
    await using var source = new WithForeignKeyContext(args[0], directory);
    Console.WriteLine("DDL " + source.Database.GenerateCreateScript());
    await source.Database.EnsureCreatedAsync(deadline.Token);
    var authorId = Guid.NewGuid();
    source.Authors.Add(new Author { Id = authorId });
    source.Books.Add(new Book { Id = Guid.NewGuid(), AuthorId = authorId });
    await source.SaveChangesAsync(deadline.Token);

    // 独立上下文类型确保目标模型确实没有外键，不复用来源模型缓存。
    await using var target = new WithoutForeignKeyContext(args[0], directory);
    var targetModel = target.GetService<IDesignTimeModel>().Model;
    var commands = source.GetService<IMigrationsSqlGenerator>().Generate(
        [new DropForeignKeyOperation
        {
            Table = "A06Books", Name = "FK_A06_Book_Author"
        }], targetModel);
    if (commands.Count is < 1 or > 16)
        throw new InvalidOperationException("Expected one bounded, nonempty drop command list.");
    foreach (var command in commands)
    {
        deadline.Token.ThrowIfCancellationRequested();
        Console.WriteLine("MIGRATION " + command.CommandText);
        await source.Database.ExecuteSqlRawAsync(command.CommandText, deadline.Token);
    }
    Console.WriteLine("DROP COMMAND EXECUTION PASSED");
    source.Books.Add(new Book { Id = Guid.NewGuid(), AuthorId = Guid.NewGuid() });
    await source.SaveChangesAsync(deadline.Token);
    Console.WriteLine("ORPHAN INSERT PASSED AFTER CONSTRAINT REMOVAL");
    result = 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
}
finally
{
    try
    {
        var expected = Path.GetFullPath(Path.Combine(parent, "a06-fk-" + owner));
        if (!string.Equals(expected, Path.GetFullPath(directory), StringComparison.OrdinalIgnoreCase) ||
            File.GetAttributes(directory).HasFlag(FileAttributes.ReparsePoint) ||
            await File.ReadAllTextAsync(Path.Combine(directory, ".owner")) != owner)
            throw new InvalidOperationException("Refusing cleanup without matching ownership.");
        Directory.Delete(directory, recursive: true);
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine("RESOURCE CLEANUP FAILED " + exception);
        result = 1;
    }
}
return result;

internal abstract class BookContext(string provider, string directory) : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (provider == "Sqlite")
            options.UseSqlite("Data Source=" + Path.Combine(directory, "reference.db") + ";Pooling=False;Default Timeout=2");
        else
            options.UseSonnetDB("Data Source=" + Path.Combine(directory, "sonnetdb"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Author>(entity =>
        {
            entity.ToTable("A06Authors");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).ValueGeneratedNever();
        });
        builder.Entity<Book>(entity =>
        {
            entity.ToTable("A06Books");
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Id).ValueGeneratedNever();
        });
    }
}

internal sealed class WithForeignKeyContext(string provider, string directory)
    : BookContext(provider, directory)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Book>().HasOne<Author>().WithMany()
            .HasForeignKey(row => row.AuthorId).OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_A06_Book_Author");
    }
}

internal sealed class WithoutForeignKeyContext(string provider, string directory)
    : BookContext(provider, directory);

internal sealed class Author { public Guid Id { get; set; } }
internal sealed class Book { public Guid Id { get; set; } public Guid AuthorId { get; set; } }
```

```text
dotnet run --project Repro.csproj -c Release -- Sqlite
dotnet run --project Repro.csproj -c Release -- SonnetDB
```

### 期望修复与验收

1. Provider 的 CREATE TABLE 保留 EF 命名外键；查询实际 schema 时名字与模型一致。
2. 标准 `DropForeignKeyOperation` 删除该外键，实际允许目标模型允许的数据；验证从有约束到无约束再到替换约束的迁移。
3. 针对不存在的约束明确说明/校验行为，避免迁移被记为成功而预期约束仍留存。即使引擎选择允许不存在名字的 DROP，Provider 仍须使用实际一致的名字。
4. 初始迁移、后续升级、回退、重启后的 schema/约束语义分别通过；公开修复包后重复相同复现。

### 与已有 issue 的边界

#188 讨论引擎命名外键语法，#250 讨论目录能力；本例通过公开 EF Provider 暴露 `HasConstraintName`→建表→后续删除的名称传递与结果一致性问题，不以缺少基础语法重复提报。已由主任务独立回读查重；本正文不推断当前引擎源码等同于公开包。




本正文中的独立代码已用公开固定包构建，0 警告/0 错误，并分别执行两个 Provider。命名外键复现的 SQLite 通过、SonnetDB 在 DROP 返回成功后仍报外键冲突；幂等脚本的 SQLite 明确拒绝选项，SonnetDB 首轮成功、重放失败。对应独立回执前缀为 artifacts/checks/a06-gap-20261011-standalone-foreign-key- 与 standalone-idempotent-。仅验证本地嵌入式，真实 Server 与完整 BookStore 尚未验收。
