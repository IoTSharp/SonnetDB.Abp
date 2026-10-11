# [A06 / EF Core] Idempotent 迁移脚本缺少历史保护，重放失败

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

公开 Provider 接受 `IMigrator.GenerateScript(options: Idempotent | NoTransactions)`，生成的脚本只对历史表使用 IF NOT EXISTS，没有按已应用 migration 的历史行保护业务 DDL/历史插入。实际首次执行成功，第二次执行同一脚本时在 CREATE TABLE 报“已存在”。

这影响把同一发布脚本重复部署到已升级数据库的合同，不表示初次迁移失败，也不等于连续 `Database.MigrateAsync` 两次失败。Provider 可以明确声明不支持 Idempotent 并在生成阶段拒绝，不能接受该选项而返回普通脚本。

### 已验证结果

环境和固定包版本同上。本 probe 使用 `Idempotent | NoTransactions`，对**已知仅含三条简单语句、无字符串内分号**的最小脚本逐条执行，避免把整段 batch 的执行方式误判为幂等问题；这不是通用 SQL 脚本分割器。

SonnetDB 实际生成并接受：

```sql
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"
    ("MigrationId" STRING NOT NULL, "ProductVersion" STRING NOT NULL,
     PRIMARY KEY ("MigrationId"));

CREATE TABLE "A06MigrationProbe" ("Id" INT NOT NULL, PRIMARY KEY ("Id"));

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('202610110002_A06Minimal', '10.0.12');
```

第一次完整执行日志包含 `FIRST SCRIPT EXECUTION PASSED`；第二次实际退出 1：

```text
System.InvalidOperationException: table 'A06MigrationProbe' 已存在。
```

SQLite 对照在 GenerateScript 阶段明确拒绝，而不是返回误导性脚本：

```text
System.NotSupportedException:
Generating idempotent scripts for migrations is not currently supported for SQLite.
```

本地完整日志为 `artifacts/checks/a06-gap-20261011-runtime-sonnetdb-idempotent-script.stdout.log` 与对应 `.stderr.log`；SQLite 对照为同前缀的 `runtime-sqlite-idempotent-script` 两份日志。失败保持原失败，专属数据库清理已完成。

### 独立 Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

if (args.Length != 1 || args[0] is not ("Sqlite" or "SonnetDB"))
{
    Console.Error.WriteLine("Usage: dotnet run -- Sqlite|SonnetDB");
    return 2;
}

var owner = Guid.NewGuid().ToString("N");
var parent = Path.GetFullPath(Environment.CurrentDirectory);
var directory = Path.Combine(parent, "a06-script-" + owner);
Directory.CreateDirectory(directory);
await File.WriteAllTextAsync(Path.Combine(directory, ".owner"), owner);
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(25));
var result = 1;
try
{
    await using var context = new ScriptContext(args[0], directory);
    var script = context.GetService<IMigrator>().GenerateScript(
        options: MigrationsSqlGenerationOptions.Idempotent | MigrationsSqlGenerationOptions.NoTransactions);
    Console.WriteLine("IDEMPOTENT SCRIPT " + script);
    // 此固定迁移没有字符串内分号；最多16条语句、两轮执行，不用于一般SQL解析。
    var statements = script.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    if (statements.Length is < 1 or > 16)
        throw new InvalidOperationException("Expected a bounded, nonempty minimal script.");
    for (var round = 0; round < 2; round++)
    {
        foreach (var statement in statements)
        {
            deadline.Token.ThrowIfCancellationRequested();
            await context.Database.ExecuteSqlRawAsync(statement, deadline.Token);
        }
        Console.WriteLine(round == 0 ? "FIRST SCRIPT EXECUTION PASSED" : "SECOND SCRIPT EXECUTION PASSED");
    }
    var applied = await context.GetService<IHistoryRepository>().GetAppliedMigrationsAsync(deadline.Token);
    if (applied.Count != 1 || applied[0].MigrationId != "202610110002_A06Minimal")
        throw new InvalidOperationException("Script replay must retain exactly one migration history row.");
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
        var expected = Path.GetFullPath(Path.Combine(parent, "a06-script-" + owner));
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

internal sealed class ScriptContext(string provider, string directory) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (provider == "Sqlite")
            options.UseSqlite("Data Source=" + Path.Combine(directory, "reference.db") + ";Pooling=False;Default Timeout=2");
        else
            options.UseSonnetDB("Data Source=" + Path.Combine(directory, "sonnetdb"));
    }
}

[DbContext(typeof(ScriptContext))]
[Migration("202610110002_A06Minimal")]
internal sealed class MinimalMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("A06MigrationProbe",
            columns: table => new { Id = table.Column<long>(nullable: false) },
            constraints: table => table.PrimaryKey("PK_A06MigrationProbe", row => row.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropTable("A06MigrationProbe");
}
```

```text
dotnet run --project Repro.csproj -c Release -- Sqlite
dotnet run --project Repro.csproj -c Release -- SonnetDB
```

### 期望修复与验收

1. 若支持 Idempotent，按历史行保护每条 migration 的 DDL 与 history 写入；空库执行一次、同脚本重放、已有前序 migration 后执行、重启后再执行均保留正确 schema 和单条历史行。
2. 若不支持，在 GenerateScript 阶段以明确异常拒绝 Idempotent；不能把普通脚本标作幂等脚本返回。
3. `Idempotent` 与 `Idempotent | NoTransactions` 分别验收；修复不依赖关闭事务。另独立验证 `MigrateAsync` 的顺序重跑，不能用其 PASS 代替脚本合同。
4. 用公开 NuGet 修复版本复测相同脚本与真实升级流程，再声明对应范围支持；当前只验证本地，不推导 Server 或完整应用。

### 与已有 issue 的边界

#227 讨论 DDL 事务。本复现明确使用 NoTransactions，第一轮成功，第二轮失败原因是迁移历史保护缺失；不重复提交 DDL 原子性或恢复问题。与 #266 JSON、命名 FK 的名称合同也分别立项。已独立回读查重，未发现同一 EF 幂等脚本问题。



本正文中的独立代码已用公开固定包构建，0 警告/0 错误，并分别执行两个 Provider。命名外键复现的 SQLite 通过、SonnetDB 在 DROP 返回成功后仍报外键冲突；幂等脚本的 SQLite 明确拒绝选项，SonnetDB 首轮成功、重放失败。对应独立回执前缀为 artifacts/checks/a06-gap-20261011-standalone-foreign-key- 与 standalone-idempotent-。仅验证本地嵌入式，真实 Server 与完整 BookStore 尚未验收。
