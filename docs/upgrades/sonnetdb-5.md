# SonnetDB 5.0 升级准备与验收门禁（A06）

本分支将 `SonnetDB.EntityFrameworkCore` 固定为 5.0.0，将 EF Core 与对应 Microsoft 依赖对齐至 10.0.14，并集中约束 Pkcs/Xml 补丁版本。NuGet 审计、warnings-as-errors、ABP 10.7.0 和原业务保持启用。

2026-11-04 的私有候选已通过完整 restore、格式检查、Release 构建和原始 22 项测试，包含真实嵌入式存储、应用账户 HTTP、CRUD、应用重建读取及 IdentityUserStore Passkey。该结果使用明确记录来源与 DLL 身份的混合候选包。**公开 NuGet 5.0.0、最终统一源码提交与远端 CI 复验仍待完成，本升级准备不得据此提前合入主线。** ABP 适配层本身也未发布 NuGet 包。

## 日期类型与新增迁移

5.0 Provider 的默认关系型 `DateTime` 使用 `DATETIME2`，默认 `DateTimeOffset` 使用 `DATETIMEOFFSET`，均按 UTC 100ns ticks 保存；DateTimeOffset 读回为零 offset。显式旧 `DATETIME` 保持整毫秒合同，亚毫秒参数会明确拒绝，应用不能通过截断业务 Clock 绕过。

BookStore 保留 `20261013155746_InitialSonnetDB`，随后应用标准生成的 `20261103161631_NativeDateTimeTicks5`：

- 43 个 DateTime 列从 `DATETIME` 演进为 `DATETIME2`。
- 4 个 DateTimeOffset 列从 `DATETIME` 演进为 `DATETIMEOFFSET`。
- Designer 与 ModelSnapshot 同步更新；标准 EF 工具确认没有待迁移的模型变更。
- 原业务、Clock、历史初始迁移、外键和模型警告策略保留。

应用必须消费包含此次非主键日期列演进修复的完整 SonnetDB 5.0 包组合。私有复验曾暴露 `AbpOrganizationUnits` 自引用外键的 native schema 迁移边界，现有 Core 修复保留迁移 journal、格式 floor 和外键校验，完整 22 项已恢复。新增迁移没有通过删除外键绕开该边界。

这里的精度合同针对关系日期列；时序 measurement time 应按其独立精度合同使用。

## 升级执行顺序

1. 先取得本说明末尾要求的公开包复验结果，并核对发布版本与来源身份。
2. 停止应用写入，关闭数据库连接，保存升级前的完整数据库目录备份及其哈希。先在备份副本演练迁移和读回。
3. 在运行环境配置 `Database__Provider=SonnetDB` 和指向该数据库目录的 `ConnectionStrings__Default`，完成依赖还原和 Release 构建。
4. 执行项目已有的标准迁移程序，让 EF 按历史顺序执行初始迁移及新增日期迁移。对已部署数据库不会重写或重新生成初始迁移。
5. 检查两条迁移历史、再次初始化、账户登录、日期值、CRUD、Passkey 与应用重建读取，再恢复业务写入。

```text
dotnet restore SonnetDB.Abp.slnx
dotnet build SonnetDB.Abp.slnx -c Release --no-restore
dotnet run --project samples/BookStore/src/Acme.BookStore.DbMigrator -c Release --no-build
```

私有测试覆盖标准 BookStore 迁移与应用旅程，也独立验证旧整毫秒日期行升级后保留原值。完整客户存量库、生产负载、物理掉电和跨模型恢复须按实际部署另行演练；不能从这些测试推导其已经通过。

## 回退边界

原始旧日期可升级为原生 ticks；一旦应用写入亚毫秒日期，就不能假设 `Down` 到旧 `DATETIME` 无损。回退应停止新版本，恢复升级前已核验的备份，并使用与备份匹配的旧应用和包组合。保留备份，直到升级后的实际业务验收完成。

## 合入与公开消费门禁

SonnetDB 5.0 正式公开后，用全新独占 NuGet 缓存、仅公开来源且空 fallback 还原；核对 SonnetDB EF/Core/Data/Protocol 的版本、源码提交与实际加载 DLL，并重新执行仓库 CI 的前端资源准备及以下四项：

```text
dotnet restore SonnetDB.Abp.slnx
dotnet format SonnetDB.Abp.slnx --verify-no-changes --no-restore --severity warn
dotnet build SonnetDB.Abp.slnx -c Release --no-restore
dotnet test SonnetDB.Abp.slnx -c Release --no-build --no-restore
```

测试必须实际发现并通过原 22 项，零失败、零跳过。构建、TRX、公开包身份与远端 CI 分别记录；此前私有候选的原失败及成功回执保留。完成这些条件后再评审本分支合入，适配层发布仍单独决定。远程 SonnetDB Server、全部 ABP Administration 模块及其它模型的验收按 ROADMAP 各自推进。
