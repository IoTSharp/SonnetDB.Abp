# [A06 / EF Core] JSON 列类型映射缺失阻塞 ABP Identity Passkey 和官方 BookStore 迁移

## 2026-10-14 公开修复包消费

已固定 NuGet.org 的 `SonnetDB.EntityFrameworkCore 4.5.0-preview.ef266.5`，引擎来源提交为 `1af23d8f299febfdf66b040ab3419980396acf46`，相关代码由 SonnetDB #306 正常合入 main。使用全新公共包缓存和仅含 NuGet.org 的配置还原；公开包有效载荷逐项匹配批准的 CI 原包，并通过仓库签名验证，未引用引擎源码或私有feed。

原 Complex/Owned × SQLite/SonnetDB 四项模型及完整 BookStore 建表脚本检查5/5通过。标准 `InitialSonnetDB` 迁移、Designer、模型快照已纳入独立迁移项目；真实账户 HTTP 与 IdentityUserStore Passkey 存储回归纳入消费者测试项目，各1/1通过，保留官方业务、模块、授权和事务。

完整HTTP使用真实ASP.NET应用的TestServer；Passkey回归验证存储及应用重建，未执行浏览器WebAuthn仪式。数据层实际REST/h2c服务及最终公开包接受由引擎侧独立记录；公共消费者提交/CI完成后才更新 #266 的关闭状态。A06全部Administration、其他问题、升级与故障恢复不因此视为完成。以下4.0.0失败均为历史实测，不删除、不改写。

## 需求与范围

本需求来自 SonnetDB.Abp 的 A06：基于 ABP 官方 BookStore MVC/EF Core 完整示例，保留 Identity、Account、权限、设置、租户、审计与 OpenIddict 等标准模块，通过适配包切换数据库。示例来源为 abpframework/abp-samples，提交 97bc6ddfbc491843ddcd101342ce10e088c1d0d6，目录 BookStore-Mvc-EfCore。

SQLite 可以对同一官方模型生成标准初始迁移。SonnetDB 在迁移生成的关系模型构建阶段就失败，尚未执行数据库 SQL。本仓库不删除 Passkey/Identity、不修改 ABP 框架、不改 NuGet 缓存，也不在适配层替代引擎 Provider 的类型映射。

## 固定环境

- Windows、PowerShell 7.6.6、.NET SDK 10.0.401，net10.0。
- Volo.Abp 10.7.0；Microsoft.EntityFrameworkCore / Relational / Sqlite / Design 10.0.12。
- 公开 NuGet SonnetDB.EntityFrameworkCore 4.0.0，无 SonnetDB 引擎源码引用。
- 已安装 dotnet-ef 10.0.5 用于生成迁移；工具提示版本较低，但不依赖 EF CLI 的纯程序最小复现同样失败，因此不能由升级 CLI 推断此缺口消失。
- 仅验证本地 Provider 模型；不推导 Server、其他模型或完整 Demo 的兼容性。

## 最小复现（无需 ABP）

SonnetDB.Abp 仓库中的 samples/SonnetDB.Abp.JsonColumnRepro 使用同一模型与两个固定 Provider。只构造标准迁移需要的设计时关系模型，无数据库文件与建表操作：

```csharp
builder.Entity<Passkey>()
    .ComplexProperty(passkey => passkey.Data, data => data.ToJson());
var relationalModel = context.GetService<IDesignTimeModel>()
    .Model.GetRelationalModel();
```

```text
dotnet restore SonnetDB.Abp.slnx
dotnet build samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-restore
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- Sqlite
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- SonnetDB
```

SQLite 实际：退出 0，`Sqlite: Passkey.Data store type = TEXT`。

也可独立建立空目录，无需获取 SonnetDB.Abp 的未提交工作区。保存 Repro.csproj 与 Program.cs，执行 `dotnet run -- Sqlite` 和 `dotnet run -- SonnetDB`：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SonnetDB.EntityFrameworkCore" Version="4.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.12" />
  </ItemGroup>
</Project>
```

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

var options = new DbContextOptionsBuilder<ReproContext>();
if (args.Single() == "Sqlite")
    options.UseSqlite("Data Source=:memory:");
else
    options.UseSonnetDB("Data Source=json-column-repro");
using var context = new ReproContext(options.Options);
var model = context.GetService<IDesignTimeModel>().Model.GetRelationalModel();
Console.WriteLine(model.Tables.Single().Columns.Single(x => x.Name == "Data").StoreType);

internal sealed class ReproContext(DbContextOptions<ReproContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder) =>
        builder.Entity<Passkey>().ComplexProperty(x => x.Data, data => data.ToJson());
}
internal sealed class Passkey
{
    public Guid Id { get; set; }
    public PasskeyData Data { get; set; } = new();
}
internal sealed class PasskeyData
{
    public string CredentialId { get; set; } = string.Empty;
}
```

SonnetDB 实际：退出 1，`InvalidOperationException`：

```text
The store type 'null' specified for JSON column 'Data' in table 'Passkey'
is not supported by the current provider.
JSON columns require a provider-specific JSON store type.
```

调用链从 `RelationalModel.CreateContainerColumn` 经关系模型初始化到 `GetRelationalModel()`，未执行 SQL。

## 官方应用复现

```text
dotnet ef migrations add InitialSonnetDB --project samples/BookStore/src/Acme.BookStore.SonnetDB.Migrations --startup-project samples/BookStore/src/Acme.BookStore.DbMigrator --context BookStoreDbContext --configuration Release --no-build --output-dir Migrations -- --Database:Provider SonnetDB --ConnectionStrings:Default "Data Source=./artifacts/bookstore-sonnetdb"
```

实际退出 1，同样的异常发生在 `Volo.Abp.Identity.IdentityUserPasskey` 的 `Data` JSON 列，调用链进入 `MigrationsScaffolder.ScaffoldMigration`。切换 Sqlite 与 SQLite 的迁移程序集后，初始迁移生成退出 0。日志分别保留在本地忽略的 artifacts/checks/a06-sonnetdb-migration-generate.*、a06-sqlite-migration-generate-3.*、a06-json-repro-sqlite.*、a06-json-repro-sonnetdb.*。

## 需要 SonnetDB 实现与验收

1. Provider 给 EF Core 10 的 `ToJson` 提供明确的 JSON 容器存储类型和对应类型映射，含本复现的 ComplexProperty，以及 ABP Identity Passkey 实际使用的映射方式。单独向模型添加 HasColumnType 不是完整实现；JSON 参数与查询也必须有相应合同。
2. 最小复现设计时/运行时模型、官方 BookStore 标准初始迁移和重复迁移通过。
3. 真正支持 JSON 聚合插入、读取、更新、删除、空值、数组、嵌套值、参数绑定、Unicode 与转义；支持应用实际使用的 JSON path 查询，并验证事务回滚、重启持久化。
4. 使用未修改 ABP 模块的 Passkey 存储与账户旅程做集成回归；SQLite 同模型对照。能力先分别验收本地和真实 Server，再声明各范围支持。
5. 通过公开 NuGet 发布修复版本，SonnetDB.Abp 固定版本后重跑同一复现和完整应用。不将源码修复、单次建模成功或 issue 关闭当作完整兼容。

这属于通用 EF Provider JSON 能力需求；不要求在本仓库研发或复制 SonnetDB 引擎。2026-10-09 已查重 Passkey、ToJson 和 JSON/EF 相关 issue；未发现相同的公开需求，SQL JSON 函数或 SqlSugar JSON 字段 issue 不等同于本 EF 容器映射缺口。

补充参照：同一官方应用的 PostgreSQL 标准迁移、重复初始化、真实 admin 登录、书籍/作者 HTTP CRUD 与分页、重建宿主后读取已通过。SQLite 完整登录独立事务遇到写锁的失败保留，未关闭安全日志/事务制造成功。

## 2026-10-11 A06 消费端复测

本轮从 NuGet flat-container 索引重新读取公开版本，最新仍为 SonnetDB.EntityFrameworkCore 4.0.0；GitHub GET 回读 #266 为 open，updated_at 为 2026-10-09T10:26:16Z。本轮没有发布修复包或修改引擎，固定依赖不变。原 main 提交 e1d363d376726693770d92645e755f45f970e955 对应的 GitHub CI 37922480045 已完成并为 success，不能替代本 issue 的完整 SonnetDB 验收。

重新执行原 Complex 复现与完整 BookStore `migrations add InitialSonnetDB`，都退出 1，仍报告 JSON 列 `Data` 的存储类型为 `null`。完整官方异常指向 `Volo.Abp.Identity.IdentityUserPasskey`；生成失败发生在建表前，未创建 SonnetDB 数据库或迁移文件。独立证据位于忽略的 artifacts/checks/a06-resume-20261011-original-complex-sonnetdb.*、a06-resume-20261011-migration-generate.*；原 2026-10-09 失败保留。

审查发现原纯 Provider 复现只覆盖 Complex JSON，而官方 Passkey 使用 Owned JSON。新增可选 `Owned` 参数，按官方 snapshot 保留 `CredentialId` 二进制主键、`TenantId`、`UserId` 外键、`Data` 中的二进制/时间/布尔/计数字段与 `Transports` 字符串数组；不引用 ABP 包、不修改官方模型、不手工设置列类型绕过 Provider。

```text
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- Sqlite Owned
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- SonnetDB Owned
```

eng/Test-A06ModelReadiness.ps1 提供 PowerShell 7 有界复测：先以 `-Micro` 执行单项 SQLite Complex，再执行完整入口，对照四项小模型并生成完整官方模型的建表 SQL。每次使用独立 label，不覆盖原证据；任何子命令失败都保留真实退出码，最终入口失败。该入口不安装 EF CLI、不执行 SQL，也不将模型/SQL 生成成功升级为迁移或应用成功。

本轮已实际执行：SQLite Complex/Owned 均退出 0，JSON 列类型 TEXT；SonnetDB Complex/Owned 与完整 BookStore 建表脚本生成均退出 1，仍是同一 JSON 容器类型缺口。完整入口汇总 `allModelsReady=false`，成功 2、失败 3、未执行 0，入口退出 1。证据为 artifacts/checks/a06-model-readiness-b5cbbadf0a7d456aac31d5504c3bcc08.json 及其中逐项关联的原日志；微试独立汇总为 a06-model-readiness-6ec01d2603a04fe3a5ddbe83d47ce9d6.json。

恢复条件仍是公开包修复并固定新版本后，先重跑两种 JSON 模型和完整官方模型，再生成独立 SonnetDB 标准迁移、实际建库/重复初始化与种子、真实 Passkey 数据读写、实际账户 HTTP CRUD/分页和宿主重启读取。本轮不预加跳过的 SonnetDB 应用测试，待实际标准迁移可执行后再复用已有完整 HTTP 旅程。
