# [A06 / EF Core] 参数化 string.Contains 未保持字面语义：%/_ 误匹配全部，反斜杠漏匹配

## 问题与实际应用影响

公开 NuGet `SonnetDB.EntityFrameworkCore 4.0.0` 将 `author.Name.Contains(filter)` 的参数直接拼入 SQL LIKE 模式。filter 为 `%` 或 `_` 时匹配全部五行；filter 为单个反斜杠时漏掉确实含该字符的行。同版本 EF Core 的 SQLite 对照对三种参数都返回唯一正确行，计数均为 1。

这是已执行本地 Provider 查询的结果错误，不是仅根据源码提出的候选风险，也不依赖 JSON issue #266。本需求来自完整官方 BookStore 的作者列表与总数路径，应用不应自行转义 filter 来补偿 Provider；LINQ `string.Contains` 的参数在此表示字面子串。

实际消费路径：

- [EfCoreAuthorRepository.GetListAsync](https://github.com/IoTSharp/SonnetDB.Abp/blob/main/samples/BookStore/src/Acme.BookStore.EntityFrameworkCore/Authors/EfCoreAuthorRepository.cs)：非空白 filter 时 `author.Name.Contains(filter)`，随后 OrderBy、Skip/Take。
- [AuthorAppService.GetListAsync](https://github.com/IoTSharp/SonnetDB.Abp/blob/main/samples/BookStore/src/Acme.BookStore.Application/Authors/AuthorAppService.cs)：非 null filter 时另用相同 Contains 谓词查询总数。

来源为固定 abp-samples 提交 `97bc6ddfbc491843ddcd101342ce10e088c1d0d6` 的 BookStore-Mvc-EfCore，ABP 10.7.0。本复现无需 ABP、完整 Demo 或本仓库未提交文件，不改变官方授权、事务、安全日志和 Passkey 模型。

## 实测环境

- 2026-10-11，Windows、PowerShell 7.6.6、.NET SDK 10.0.401、net10.0。
- `SonnetDB.EntityFrameworkCore 4.0.0`，来自公开 NuGet，无引擎源码 ProjectReference、缓存修改或自编译 Provider 替换。
- `Microsoft.EntityFrameworkCore`、`Relational`、`Sqlite` 均为 `10.0.12`。
- SonnetDB 使用 `Data Source=<本次 GUID 专属目录>/sonnetdb`，嵌入式本地 Provider；SQLite 使用同任务独立文件。
- 数据初始化、参数查询与计数均已实际执行。六个独立场景分别创建并清理所属资源。

## 预期与实际

固定输入为 `100% Real`、`100X Real`、`A_B`、`AXB`、`path\name` 五个名字。

| filter（运行时参数） | 预期唯一名字 / Count | SonnetDB 4.0.0 实际 | SQLite 10.0.12 实际 |
|---|---|---|---|
| `%` | `100% Real` / 1 | 五个名字全部返回 / 5 | `100% Real` / 1 |
| `_` | `A_B` / 1 | 五个名字全部返回 / 5 | `A_B` / 1 |
| 单个反斜杠 `\` | `path\name` / 1 | 空列表 / 0 | `path\name` / 1 |

列表和 Count 都错误；查询本身执行了，失败由结果断言报告，不能仅用命令能够执行或 SQL 可生成判断通过。

SonnetDB 已记录的实际查询 SQL（filter 分别为 `%`、`_`、单个反斜杠）：

```sql
-- @filter='%'
SELECT "a"."Id", "a"."BirthDate", "a"."Name"
FROM "A06Authors" AS "a"
WHERE "a"."Name" LIKE concat(concat('%', @filter), '%')
ORDER BY "a"."Name"
```

其余两场景的 SQL 结构相同，仅参数变为 `@filter='_'` 或 `@filter='\'`。这里的参数注释表示实际参数值，不是手工展开后的 SQL 字面量。

SQLite 同模型对照已记录：

```sql
.param set @filter '%'

SELECT "a"."Id", "a"."BirthDate", "a"."Name"
FROM "A06Authors" AS "a"
WHERE instr("a"."Name", @filter) > 0
ORDER BY "a"."Name"
```

列表 `ToQueryString()` 是上述 SQL 的来源；Count 是对同一 IQueryable 另行实际执行并记录的结果，没有将未捕获的 Count SQL 当作日志证据。

## 完整独立最小复现

在仓库外的空目录保存以下两个文件即可；不需要取得 SonnetDB.Abp 的未提交文件。代码从已执行合同 probe 裁剪为仅 Author 一张表，保留同一字段类型、参数 Contains、列表和 Count，删除与此缺陷无关的 Books 表。该独立版本已使用上述公开包重新构建，0 警告/0 错误；六项实际执行重现相同结果，SQLite 三项通过，SonnetDB 三项仍失败。独立回执前缀为 artifacts/checks/a06-gap-20261011-standalone-contains-。

`ContainsRepro.csproj`：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SonnetDB.EntityFrameworkCore" Version="4.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="10.0.12" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.12" />
  </ItemGroup>
</Project>
```

`Program.cs`：

```csharp
using Microsoft.EntityFrameworkCore;

if (args.Length != 2 || args[0] is not ("SonnetDB" or "Sqlite") ||
    args[1] is not ("percent" or "underscore" or "backslash"))
{
    Console.Error.WriteLine("Usage: dotnet run -- SonnetDB|Sqlite percent|underscore|backslash");
    return 2;
}

var filter = args[1] switch
{
    "percent" => "%",
    "underscore" => "_",
    _ => "\\"
};
var expected = args[1] switch
{
    "percent" => "100% Real",
    "underscore" => "A_B",
    _ => "path\\name"
};
var owner = Guid.NewGuid().ToString("N");
var root = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-contains-repro"));
var directory = Path.GetFullPath(Path.Combine(root, owner));
var marker = Path.Combine(directory, ".owner");
var created = false;
var result = 1;
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(25));
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    deadline.Cancel();
};

try
{
    if (Directory.Exists(directory))
        throw new InvalidOperationException("Refusing to reuse an existing directory.");
    Directory.CreateDirectory(directory);
    created = true;
    await File.WriteAllTextAsync(marker, owner, deadline.Token);
    await using var context = new ReproContext(args[0], directory);
    Console.WriteLine("DDL " + context.Database.GenerateCreateScript());
    await context.Database.EnsureCreatedAsync(deadline.Token);
    context.Authors.AddRange(
        new Author { Id = Guid.NewGuid(), Name = "100% Real", BirthDate = new DateTime(1980, 1, 1) },
        new Author { Id = Guid.NewGuid(), Name = "100X Real", BirthDate = new DateTime(1981, 1, 1) },
        new Author { Id = Guid.NewGuid(), Name = "A_B", BirthDate = new DateTime(1982, 1, 1) },
        new Author { Id = Guid.NewGuid(), Name = "AXB", BirthDate = new DateTime(1983, 1, 1) },
        new Author { Id = Guid.NewGuid(), Name = "path\\name", BirthDate = new DateTime(1984, 1, 1) });
    await context.SaveChangesAsync(deadline.Token);
    context.ChangeTracker.Clear();

    var query = context.Authors.Where(author => author.Name.Contains(filter))
        .OrderBy(author => author.Name);
    Console.WriteLine("QUERY " + query.ToQueryString());
    var names = await query.Select(author => author.Name).ToListAsync(deadline.Token);
    var count = await query.CountAsync(deadline.Token);
    Console.WriteLine("MATCHES " + string.Join(" | ", names) + "; COUNT " + count);
    if (names.Count != 1 || count != 1 || names[0] != expected)
        throw new InvalidOperationException("Contains must match the literal parameter character.");
    Console.WriteLine("PASS " + args[0] + " " + args[1]);
    result = 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
}
finally
{
    if (created)
    {
        try
        {
            using var cleanupDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (!string.Equals(directory, Path.GetFullPath(Path.Combine(root, owner)),
                    StringComparison.OrdinalIgnoreCase) ||
                File.GetAttributes(directory).HasFlag(FileAttributes.ReparsePoint) ||
                !File.Exists(marker) ||
                await File.ReadAllTextAsync(marker, cleanupDeadline.Token) != owner)
                throw new InvalidOperationException("Refusing cleanup of unverified resources: " + directory);
            Directory.Delete(directory, recursive: true);
            Console.WriteLine("CLEANED " + directory);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("RESOURCE CLEANUP FAILED " + exception);
            result = 1;
        }
    }
}
return result;

internal sealed class ReproContext(string provider, string directory) : DbContext
{
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (provider == "Sqlite")
            options.UseSqlite("Data Source=" + Path.Combine(directory, "reference.db") + ";Pooling=False");
        else
            options.UseSonnetDB("Data Source=" + Path.Combine(directory, "sonnetdb"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Author>(entity =>
        {
            entity.ToTable("A06Authors");
            entity.HasKey(author => author.Id);
            entity.Property(author => author.Id).ValueGeneratedNever();
            entity.Property(author => author.Name).IsRequired().HasMaxLength(64);
        });
    }
}

internal sealed class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
}
```

依次运行以下有限的六个场景，不循环重试；正式执行应由有界命令 runner 限制每个运行进程，并保留真实退出码及资源清理结果：

```text
dotnet restore ContainsRepro.csproj
dotnet build ContainsRepro.csproj -c Release --no-restore
dotnet run --project ContainsRepro.csproj -c Release --no-build --no-restore -- SonnetDB percent
dotnet run --project ContainsRepro.csproj -c Release --no-build --no-restore -- SonnetDB underscore
dotnet run --project ContainsRepro.csproj -c Release --no-build --no-restore -- SonnetDB backslash
dotnet run --project ContainsRepro.csproj -c Release --no-build --no-restore -- Sqlite percent
dotnet run --project ContainsRepro.csproj -c Release --no-build --no-restore -- Sqlite underscore
dotnet run --project ContainsRepro.csproj -c Release --no-build --no-restore -- Sqlite backslash
```

## 修复与验收要求

1. EF Provider 对运行时参数 `string.Contains` 保持字面子串语义，正确处理 `%`、`_` 与反斜杠；保留参数化，不要求消费方预转义或改为手工 SQL。
2. 上述三个场景在公开修复包上，列表和 Count 均只返回预期唯一行；单个字符、组合字符串、常量与参数分别覆盖。补充空字符串、空值/可空列和 Unicode 时，以明确的 .NET/EF 查询合同为准。
3. `StartsWith`/`EndsWith` 若共用同一模式构造逻辑，应增加对应字面字符回归；本次没有执行这两个方法，不能把它们记为已复现失败。
4. 本地 SQL 实际执行与真实 Server 分别验收。当前六个结果只来自嵌入式本地 Provider，Server、远程传输、其它数据模型、完整 BookStore SonnetDB 账户旅程未验。
5. 发布修复后的公开 NuGet 包，消费端固定版本后重跑相同三个字符的列表/Count 对照，再复测原官方作者筛选路径。源码修改或单次 SQL 生成不能代替公开包执行证据。

修复方案可使用字面子串运算或正确的参数模式转义，但需要与引擎 LIKE/ESCAPE 合同一致；本文不根据当前工作树实现来指定必须采用的 SQL 函数。

## 查重与证据归属

已做有界 issue 查重。已有 [#174](https://github.com/IoTSharp/SonnetDB/issues/174) 讨论 SQL 字符串函数能力，本项是 EF `string.Contains` 参数被翻译为 LIKE 后改变字面语义的实测缺陷，不重复申请同一 SQL 函数。JSON #266 为完整模型容器类型映射，是另一独立问题。查重不是穷尽搜索，如已有完全相同的 EF 参数转义问题，应合并到对应 issue。

本地复现日志分别为以下前缀的 `.stdout.log`/`.stderr.log`，由消费端保留，不要求维护者取得这些文件才能运行上述完整代码：

```text
artifacts/checks/a06-gap-20261011-query-sonnetdb-contains-percent
artifacts/checks/a06-gap-20261011-query-sonnetdb-contains-underscore
artifacts/checks/a06-gap-20261011-query-sonnetdb-contains-backslash
artifacts/checks/a06-gap-20261011-query-sqlite-contains-percent
artifacts/checks/a06-gap-20261011-query-sqlite-contains-underscore
artifacts/checks/a06-gap-20261011-query-sqlite-contains-backslash
```

六份 stdout 均记录独立 GUID 目录清理；SonnetDB 的错误列表/计数原样保留。上述完整独立代码已经实际构建和运行，失败记录原样保留；没有修改 Provider、引擎或官方业务来绕过。
