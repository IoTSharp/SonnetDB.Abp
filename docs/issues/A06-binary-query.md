# [EF Core/ABP] 二进制主键参数比较失败，阻塞 Identity Passkey 凭据查询

2026-10-11，公开 NuGet `SonnetDB.EntityFrameworkCore` **4.0.0** 的嵌入式数据库已实测：`byte[]` 主键可以生成 BLOB 列、建表并写入，但通过二进制参数读取时抛出 `InvalidOperationException: 无法比较 Byte[] 与 String。`。同一模型和查询使用 SQLite 均通过。

两个独立触发方式分别为普通 `CredentialId == byte[]` 条件，以及 ABP 实际使用的关联 `Any` + `SequenceEqual`。模型没有 JSON 字段、`ToJson` 或 ABP 依赖；这与 [#266 JSON 容器类型映射](https://github.com/IoTSharp/SonnetDB/issues/266) 是不同缺口，不需要等待该 JSON 模型通过才能复现。

## 实际应用触发

SonnetDB.Abp 的 A06 沿用官方 BookStore 的 ABP Identity 模块。已核对 ABP **10.7.0** 的 `EfCoreIdentityUserRepository.FindByPasskeyIdAsync`，源码位置为：

```text
modules/identity/src/Volo.Abp.Identity.EntityFrameworkCore/Volo/Abp/Identity/EntityFrameworkCore/EfCoreIdentityUserRepository.cs
```

其核心查询按用户的 Passkeys 集合匹配凭据内容，再按用户 Id 取首项：

```csharp
Users.Where(u => u.Passkeys.Any(p => p.CredentialId.SequenceEqual(credentialId)))
    .OrderBy(u => u.Id)
    .FirstOrDefaultAsync(cancellationToken);
```

下面的独立复现保留该查询形状，去掉完整 Identity 的 JSON `Data`、其他模块和业务，不替换凭据内容比较或查询授权来绕过数据库错误。它验证底层凭据读取合同，不宣称完整 Passkey 认证或 BookStore 已运行通过。

## 固定环境与已观察结果

- Windows、PowerShell 7.6.6、.NET SDK 10.0.401，目标框架 `net10.0`。
- 公开 NuGet `SonnetDB.EntityFrameworkCore` 4.0.0。
- `Microsoft.EntityFrameworkCore` / `Relational` / `Sqlite` 10.0.12；应用消费者为 ABP 10.7.0，最小复现无需 ABP。
- 使用真实本地嵌入式数据库，不使用 mock、跨仓库源码引用或修改 NuGet 缓存；远程 Server 未由本次结果验收。

| 探测 | SQLite 10.0.12 | SonnetDB EF 4.0.0 |
| --- | --- | --- |
| 普通 BLOB 主键写入后 `CredentialId == key` 查询 | 通过 | 写入后查询失败：`无法比较 Byte[] 与 String。` |
| 两个不同凭据用户的 `Passkeys.Any(p => p.CredentialId.SequenceEqual(lookup))` | 正确命中目标用户；不存在凭据不命中 | 已完成建表/种子；首次命中查询失败，未到达不存在凭据的断言 |

现有 SonnetDB 日志记录的相关建表 SQL 如下；同一探测中的其他无关表已省略。`UserId` 为 `Guid`，该 Provider 生成其 STRING 存储类型。

```sql
CREATE TABLE "A06Users" ("Id" STRING NOT NULL,
    "NormalizedUserName" STRING NOT NULL,
    PRIMARY KEY ("Id")
);

CREATE TABLE "A06UserPasskeys" ("CredentialId" BLOB NOT NULL,
    "UserId" STRING NOT NULL,
    "Name" STRING NOT NULL,
    PRIMARY KEY ("CredentialId"),
    FOREIGN KEY ("UserId") REFERENCES "A06Users" ("Id") ON DELETE CASCADE
);
```

关联查询的实际 `ToQueryString()` 输出为：

```sql
-- @lookup='0x00017F80FF'
-- @p='1'
SELECT "a"."Id", "a"."NormalizedUserName"
FROM "A06Users" AS "a"
WHERE EXISTS (
    SELECT 1
    FROM "A06UserPasskeys" AS "a0"
    WHERE "a"."Id" = "a0"."UserId" AND "a0"."CredentialId" = @lookup)
ORDER BY "a"."Id"
LIMIT @p
```

上面的参数注释是 EF 调试展示，不能仅凭 `0x...` 字符串展示推断实际 ADO 参数类型。两种查询的异常均进入 `SqlScalarComparer.Compare` / `TableSqlExecutor.EvaluateComparison`；关联查询继续经过 `RelationalSelectExecutor.TryExecuteSingleTableExists`，普通条件查询经过单表读取执行器，随后进入 EF `ExecuteReaderAsync`。共同异常为：

```text
System.InvalidOperationException: 无法比较 Byte[] 与 String。
   at SonnetDB.Sql.Execution.SqlScalarComparer.Compare(Object left, Object right)
   at SonnetDB.Sql.Execution.TableSqlExecutor.CompareScalar(Object left, Object right)
   at SonnetDB.Sql.Execution.TableSqlExecutor.EvaluateComparison(...)
```

这说明错误发生在已翻译 SQL 的实际读取执行阶段。候选范围包括 EF 二进制类型映射/参数创建、ADO 参数绑定、SQL 参数值表示及比较执行；当前证据尚未锁定哪一环节将一侧表示为字符串，不能直接宣布确定根因。

## 独立最小复现

在空目录保存以下两个文件即可运行，不需要 SonnetDB.Abp 工作区或未提交文件。正文代码已独立用公开固定包构建，0 警告/0 错误；四项实际运行中 SQLite 两项通过，SonnetDB 两项重现相同 Byte[]/String 异常。完整代码回执前缀为 artifacts/checks/a06-gap-20261011-standalone-binary-。

`Repro.csproj`：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SonnetDB.EntityFrameworkCore" Version="4.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.12" />
  </ItemGroup>
</Project>
```

`Program.cs`：

```csharp
using Microsoft.EntityFrameworkCore;

if (args.Length != 2 || args[0] is not ("Sqlite" or "SonnetDB") ||
    args[1] is not ("binary-key" or "passkey-any"))
{
    Console.Error.WriteLine("Usage: dotnet run -- Sqlite|SonnetDB binary-key|passkey-any");
    return 2;
}

var owner = Guid.NewGuid().ToString("N");
var root = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "binary-query-repro"));
var directory = Path.Combine(root, owner);
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(25));
ConsoleCancelEventHandler cancel = (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    deadline.Cancel();
};
Console.CancelKeyPress += cancel;
var result = 1;
try
{
    Directory.CreateDirectory(directory);
    await File.WriteAllTextAsync(Path.Combine(directory, ".owner"), owner, deadline.Token);
    await using var context = new ReproContext(args[0], args[1], directory);
    Console.WriteLine(context.Database.GenerateCreateScript());
    await context.Database.EnsureCreatedAsync(deadline.Token);
    byte[] key = [0, 1, 127, 128, 255];

    if (args[1] == "binary-key")
    {
        context.Passkeys.Add(new Passkey
        {
            CredentialId = key, UserId = Guid.NewGuid(), Name = "non-JSON credential"
        });
        await context.SaveChangesAsync(deadline.Token);
        context.ChangeTracker.Clear();
        var query = context.Passkeys.Where(passkey => passkey.CredentialId == key);
        Console.WriteLine(query.ToQueryString());
        var saved = await query.SingleAsync(deadline.Token);
        Require(saved.CredentialId.SequenceEqual(key), "Credential contents must roundtrip.");
    }
    else
    {
        var user = new User { Id = Guid.NewGuid(), NormalizedUserName = "PASSKEY" };
        user.Passkeys.Add(new Passkey
        {
            CredentialId = key, UserId = user.Id, Name = "target credential"
        });
        var other = new User { Id = Guid.NewGuid(), NormalizedUserName = "OTHER" };
        other.Passkeys.Add(new Passkey
        {
            CredentialId = [9, 8, 7], UserId = other.Id, Name = "different credential"
        });
        context.Users.AddRange(user, other);
        await context.SaveChangesAsync(deadline.Token);
        context.ChangeTracker.Clear();
        byte[] lookup = [0, 1, 127, 128, 255];
        var query = context.Users
            .Where(row => row.Passkeys.Any(passkey => passkey.CredentialId.SequenceEqual(lookup)))
            .OrderBy(row => row.Id);
        Console.WriteLine(query.Take(1).ToQueryString());
        Require((await query.FirstOrDefaultAsync(deadline.Token))?.Id == user.Id,
            "Lookup must select the user whose credential contents match.");
        lookup = [4, 3, 2];
        Require(await query.FirstOrDefaultAsync(deadline.Token) is null,
            "An absent credential must not match either user.");
    }
    Console.WriteLine($"PASS {args[0]} {args[1]}");
    result = 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
}
finally
{
    Console.CancelKeyPress -= cancel;
    if (Directory.Exists(directory))
    {
        try
        {
            var expected = Path.GetFullPath(Path.Combine(root, owner));
            if (!string.Equals(expected, Path.GetFullPath(directory), StringComparison.OrdinalIgnoreCase) ||
                File.GetAttributes(directory).HasFlag(FileAttributes.ReparsePoint) ||
                await File.ReadAllTextAsync(Path.Combine(directory, ".owner")) != owner)
            {
                throw new InvalidOperationException("Refusing cleanup without matching ownership.");
            }
            Directory.Delete(directory, recursive: true);
            Console.WriteLine("CLEANED " + directory);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("RESOURCE CLEANUP FAILED: " + exception);
            result = 1;
        }
    }
}
return result;

static void Require(bool condition, string message)
{
    if (!condition) { throw new InvalidOperationException(message); }
}

internal sealed class ReproContext(string provider, string scenario, string directory) : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Passkey> Passkeys => Set<Passkey>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (provider == "Sqlite")
        {
            options.UseSqlite("Data Source=" + Path.Combine(directory, "reference.db") +
                ";Pooling=False;Default Timeout=2");
        }
        else { options.UseSonnetDB("Data Source=" + Path.Combine(directory, "sonnetdb")); }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Passkey>(entity =>
        {
            entity.ToTable("A06UserPasskeys");
            entity.HasKey(row => row.CredentialId);
            entity.Property(row => row.CredentialId).HasMaxLength(1024).ValueGeneratedNever();
        });
        if (scenario == "passkey-any")
        {
            builder.Entity<User>(entity =>
            {
                entity.ToTable("A06Users");
                entity.HasKey(row => row.Id);
                entity.HasMany(row => row.Passkeys).WithOne().HasForeignKey(row => row.UserId);
            });
        }
        else { builder.Ignore<User>(); }
    }
}

internal sealed class User
{
    public Guid Id { get; set; }
    public string NormalizedUserName { get; set; } = string.Empty;
    public List<Passkey> Passkeys { get; set; } = [];
}

internal sealed class Passkey
{
    public byte[] CredentialId { get; set; } = [];
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

分别运行以下四项，每个进程只执行一个 Provider、一个场景，避免不同模型的同进程缓存干扰。程序有 25 秒取消预算及 Ctrl+C 取消入口，退出码 0 表示该项通过、1 表示失败、2 表示参数错误；失败保留为失败。

```text
dotnet run --project Repro.csproj -c Release -- Sqlite binary-key
dotnet run --project Repro.csproj -c Release -- SonnetDB binary-key
dotnet run --project Repro.csproj -c Release -- Sqlite passkey-any
dotnet run --project Repro.csproj -c Release -- SonnetDB passkey-any
```

## 期望合同与验收

1. BLOB 二进制主键使用内容比较：同内容的不同 `byte[]` 实例能够命中；不同长度/内容、不存在凭据不命中。覆盖空数组、`0x00`、高位字节与最大支持长度，明确 NULL 列/参数边界。
2. 参数化普通相等条件及 ABP 的相关 `Any` + `SequenceEqual` 均在服务器端正确执行；不把凭据变成普通字符串、不使用客户端全表扫描或关闭查询来绕过。
3. 多用户、每用户多个凭据、不同凭据和不存在凭据的正负例分别断言；验证写入、读取、更新/删除、事务回滚与关闭后重开读取。二进制主外键或索引的支持范围独立记录。
4. 对修复后的公开 NuGet 版本重跑同一 SQLite/嵌入式合同；远程 ADO/Server 使用同一内容比较合同单独验收。固定最低修复版本后，SonnetDB.Abp 再执行实际 ABP Passkey 读写与账户旅程。
5. #266 的 JSON 容器问题、其他 EF 迁移需求与本二进制查询分别跟踪；普通二进制读取成功不代表完整 JSON Passkey、真实认证或 A06 已完成。

本次已运行探测的本地忽略证据：`artifacts/checks/a06-gap-20261011-schema-sonnetdb-binary-key.{stdout,stderr}.log`、`a06-gap-20261011-query-sonnetdb-passkey-any.{stdout,stderr}.log` 及对应 SQLite 对照；两个不同用户与不存在凭据的加强探测对应 `a06-gap-20261011-refined-sonnetdb-passkey-any.*`。正文保留可独立复制的代码与实际 SQL，接收方不需要这些本地文件即可复现。原探测使用 GUID 专属目录并在 finally 核验所有权后清理；日志中的 `CLEANED` 与失败记录同时保留。
