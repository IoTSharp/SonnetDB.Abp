# SonnetDB.Abp

SonnetDB 的独立 ABP 集成仓库。首期提供 EF Core 适配模块，应用继续使用 ABP 的 DbContext、默认仓储、工作单元与数据过滤。

首期本地接线已实现：八项真实数据库集成测试通过，控制台样例可以使用 ABP 标准仓储写入和读取。测试结果与未验边界见 [兼容性记录](docs/compatibility.md)。尚未发布适配层 NuGet 包，也未提交 ABP 上游 PR。

当前主线为官方完整应用与九模型适配，顺序和表情状态见 [ROADMAP](ROADMAP.md)。已导入固定来源的 [BookStore 完整 Demo](samples/BookStore/DEMO.md)，提供 PostgreSQL 完整 HTTP 基线、SQLite 局部对照与 SonnetDB 配置切换。SonnetDB 完整模型目前受 [JSON 列 Provider 缺口](docs/issues/A06-ef-json-columns.md) 阻塞，已提交上游 #266；基础仓储通过不代表完整 Identity/Web 应用通过。

第一批 [13 个官方示例](samples/README.md) 已全部纳入仓库，先导入、后逐个迁移；新增的 12 项保留完整原始源码与许可证，尚未构建/运行或纳入根解决方案。路线图分别记录源码导入与 SonnetDB 迁移状态，完整迁移验收目前为 0/13。

## 固定版本

| 组件 | 版本 |
| --- | --- |
| .NET | 10，SDK 10.0.100 起，latestMinor |
| ABP | 10.7.0 |
| EF Core | 10.0.12 |
| SonnetDB.EntityFrameworkCore | 4.0.0，公开 NuGet 包 |

本仓库不依赖旁边 SonnetDB 的源码或未提交改动。通用引擎和 Provider 修复回 [SonnetDB](https://github.com/IoTSharp/SonnetDB) 单独处理。

## 接入

在应用的 EF Core 模块添加依赖：

```csharp
[DependsOn(typeof(SonnetDbAbpEntityFrameworkCoreModule))]
public class MyEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<MyDbContext>(options =>
            options.AddDefaultRepositories());
        Configure<AbpDbContextOptions>(options => options.UseSonnetDB());
    }
}
```

`UseSonnetDB` 位于 `Volo.Abp.EntityFrameworkCore` 命名空间，模块位于 `SonnetDB.Abp.EntityFrameworkCore`。也可使用 `options.UseSonnetDB<MyDbContext>()` 只配置指定上下文。

连接字符串沿用 ABP 的 `ConnectionStrings:Default`，例如 `Data Source=./app-data`。适配层使用 ABP 已解析的连接串，并优先复用工作单元传入的 `ExistingConnection`；不自行解析租户或选择数据库。

## 构建与样例

```sh
dotnet restore SonnetDB.Abp.slnx
dotnet build SonnetDB.Abp.slnx -c Release --no-restore
dotnet test SonnetDB.Abp.slnx -c Release --no-build --no-restore
dotnet run --project samples/SonnetDB.Abp.Sample -c Release -- ./demo-data
```

样例是真实本地数据库和 ABP `IRepository` 的控制台旅程：建立新库、事务写入、重新读取。它使用 `EnsureCreated`，不是 migrations 示范；每次运行添加一个设备。Windows 可通过 `eng/Invoke-CheckedCommand.ps1` 进行带超时和进程身份记录的验证，日志保存在忽略的 `artifacts/checks/`。

样例和测试使用 ABP 标准 Autofac 集成，以支持 DbContext 的属性注入和仓储拦截；适配包本身不强制替换应用的 DI 容器。

## 边界

- 首期以本地嵌入式关系存储为验收对象。远程 Server、独立租户数据库、标准迁移和 ABP 内置模块仍需各自验收。
- ABP Identity、Permission/Setting Management、OpenIddict 的支持需真实模块测试，不从 ASP.NET Core Identity 或仓储测试推导。
- 不将关系工作单元扩展为跨库、跨模型的原子事务；时序、向量、文档等能力后续单独集成。
- ABP/EF Core 集成使用运行时模型，不声明 Native AOT 兼容。
- 适配层许可证与正式发布策略待维护者确定；本轮不自动发布 NuGet。

项目布局与下一步见 [ROADMAP](ROADMAP.md)、[架构决策](docs/architecture.md) 和 [HANDOFF](HANDOFF.md)。
