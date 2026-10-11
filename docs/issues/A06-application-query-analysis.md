# A06 应用查询合同与独立复测边界

> 本文记录应用静态审查阶段。后续实际合同矩阵、作者筛选 #270、二进制查询 #271 与未验边界见 [A06 缺口分析](A06-sonnetdb-gap-analysis.md)；不要把本文中的候选清单当成全部已复现故障。

日期：2026-10-11（Asia/Shanghai）。范围：S01 官方 BookStore MVC/EF Core、固定 ABP 10.7.0 与公开 SonnetDB.EntityFrameworkCore 4.0.0。本文记录实际应用调用形状和独立 probe 建议，不把源码推断、还原或构建成功记为公开包数据库合同通过。

完整 SonnetDB 模型目前仍受 [JSON issue #266](https://github.com/IoTSharp/SonnetDB/issues/266) 阻塞，尚未执行完整 BookStore SonnetDB 建库、种子或账户 HTTP 旅程。本文的非 JSON 小模型只用于隔离其它 Provider/引擎合同；它们不能替代完整官方模型验收，也不要求删掉 Passkey、修改 ABP 或改变 Demo 的授权、事务和安全日志。

## 已读取的本地证据

| 回执 | 可得结论 | 不能得出的结论 |
|---|---|---|
| `artifacts/checks/a06-model-readiness-b5cbbadf0a7d456aac31d5504c3bcc08.json` | 五项已执行：SQLite Complex/Owned 成功；SonnetDB Complex/Owned 与完整官方模型建表 SQL 生成失败；`allModelsReady=false`，入口失败 | 不能计为建库、迁移执行或应用运行 |
| `artifacts/checks/a06-resume-20261011-final-test-totals.json` 与关联 TRX | 上轮最终 20/20：SonnetDB 小模型集成 8、官方 SQLite 业务 10、官方欢迎页 1、PostgreSQL 实际账户 HTTP 旅程 1，无失败/跳过 | 不能推导完整 SonnetDB BookStore 通过 |
| `artifacts/checks/a06-gap-20261011-ci-issue-review.json` | 只读有界查重已经记录相关旧 issue 与条件性重叠；`exhaustive=false` | 不能宣称没有重复 issue，也不能把候选风险记为已复现 |
| `artifacts/checks/a06-gap-20261011-probe-restore.json` | 独立合同 probe 项目还原退出 0 | 不是 probe 运行或数据库合同证据 |

本文整理时尚未读取独立合同 probe 的运行结果；后续结果应另列真实命令回执、逐项预期/实际值与失败阶段，不覆盖本轮最初失败。旧 SQLite 账户登录安全日志写锁、上轮 SQLite fixture 注册函数失败和当前 SonnetDB JSON 模型失败是三个不同范围，不能合并根因。

## 实际 A06 路径

### 1. 作者查询、唯一性检查和删除

来源：`samples/BookStore/src/Acme.BookStore.EntityFrameworkCore/Authors/EfCoreAuthorRepository.cs`、`Application/Authors/AuthorAppService.cs`、`Domain/Authors/AuthorManager.cs`。

- `FindByNameAsync(name)`：`FirstOrDefaultAsync(author => author.Name == name)`，用于新增/改名之前的业务重复检查。
- 列表：非空白参数 `filter` 时 `Name.Contains(filter)`，随后动态 `OrderBy(sorting)`、`Skip(int)`、`Take(int)`；默认按 `Author.Name` 排序。
- 总数另查：`input.Filter == null` 时全量 `CountAsync()`，其它情况为 `CountAsync(author => author.Name.Contains(input.Filter))`。probe 应同时覆盖实际计数结果与分页结果，不能只看 SQL 能生成。
- `Author` 是 `FullAuditedAggregateRoot<Guid>`，删除走软删除与过滤；Book 删除的语义不同。

列表会跳过空白 filter，但总数分支只判 null。该形状来自官方应用，不能将空白 filter 的列表/总数差异直接报为数据库错误。作者名业务重复检查也不等于数据库已有唯一索引：当前模型只对 `Author.Name` 建普通索引。

### 2. 书籍作者 JOIN、排序和分页

来源：`samples/BookStore/src/Acme.BookStore.Application/Books/BookAppService.cs`。

详情查询在 Books 与 Authors 的 `AuthorId == Id` 上 INNER JOIN，另按 Book.Id 过滤，再 `FirstOrDefaultAsync`，投影中同时读取书籍实体和作者实体。列表使用同一 JOIN，先排序、再 Skip/Take，最后 materialize；总数另走 Book 仓储 `GetCountAsync()`，不是对分页或 JOIN 结果计数。

`NormalizeSorting` 是客户端排序表达式改写：默认 `book.Name`，`authorName` 替换为 `author.Name`，其它排序加 `book.` 前缀。其中 `Contains(..., StringComparison.OrdinalIgnoreCase)` 和 `Replace` 操作排序字符串，不构成 SQL 字符串方法翻译需求。

独立 probe 应覆盖按书名/作者名排序、非零 Skip、关联详情和总数。官方列表没有自动附加唯一排序键；probe 数据应使指定主排序值唯一，不能把未指定次序的同键行排列当作数据库缺陷。当前分页参数是 int，不是 Int64 LIMIT/OFFSET 的实际 A06 基线。

### 3. 种子、账户归一化和 Identity 集合

来源：`Domain/BookStoreDataSeederContributor.cs`、`Domain/Data/BookStoreDbMigrationService.cs`，以及固定 ABP 10.7.0 的 IdentityDataSeeder、IdentityUserStore、EfCoreIdentityUserRepository 和 IdentityEfCoreQueryableExtensions。

BookStore 种子先查 Books 数量；为空才建立两名作者和两本书，并在书籍插入时 autoSave。标准 Migrator 执行完整模块种子；同一数据库重复初始化必须不重复写入。

Identity 种子先由 `LookupNormalizer.NormalizeName(adminUserName)` 在客户端生成归一化名字，再按 `NormalizedUserName == normalizedUserName` 查用户；仓储默认 includeDetails=true。标准账户查找 `IdentityUserStore.FindByNameAsync` 显式使用 includeDetails=false。两条路径均有 `OrderBy(user.Id).FirstOrDefaultAsync`，不能将客户端 Normalize 误写为数据库执行 `Normalize` 或 `ToUpper`。

Identity includeDetails 路径包含 Roles、Logins、Claims、Tokens、OrganizationUnits、PasswordHistories、Passkeys 七个集合。需要验证实际 EF SQL 的关联、子查询与多集合 materialization，不能以一个普通 User 查找成功推导全部集合加载成功。

### 4. 角色、权限与组织树

固定 ABP 10.7.0 的角色名查询包含 UserRole/Role JOIN、集合 Contains 和直接角色与组织角色的 UNION。PermissionGrant 批量读取包含 `names.Contains(grant.Name)` 以及 ProviderName、ProviderKey 条件；当前模型还含 `(TenantId, Name, ProviderName, ProviderKey)` 复合唯一索引。

组织单元 Administration 仓储使用 `OrganizationUnit.Code.StartsWith(code)` 查后代。该路径不是现有 Book/Author HTTP 旅程已覆盖的范围，应单独标记为内置模块待验。

旧 SQL Server snapshot 中的方括号过滤索引注解属于该来源模型的 Provider 形状，不能直接当成 SonnetDB 当前运行时模型要求；必须先读实际模型和生成 SQL。只验证列表/IN 结果也不能关闭复合唯一索引、NULL 与租户边界合同。

### 5. Passkey 的非 JSON 合同

官方形状包含 byte[] CredentialId 主键、最大长度 1024、TenantId 和 UserId 外键。实际 `FindByPasskeyIdAsync` 查询形状为：

```csharp
users.Where(user => user.Passkeys.Any(passkey =>
    passkey.CredentialId.SequenceEqual(credentialId)))
    .OrderBy(user => user.Id)
    .FirstOrDefaultAsync();
```

因此仅验证 BLOB 列能建成不足够。非 JSON User/Passkey 小模型可隔离二进制主键、同内容不同数组实例的参数比较、SequenceEqual 翻译和相关 EXISTS。完整 Data 的 Owned JSON roundtrip 仍归 #266；隔离 probe 的非 JSON 数据列不得回写官方 Demo 作为绕过方案。

IdentityUserToken 还使用 `(UserId, LoginProvider, Name)` 三列主键。应分别验证复合键查找和重复键拒绝，不能从普通 GUID 主键成功推导这些形状成功。

### 6. 并发戳、审计和安全日志独立事务

Book 为 `AuditedAggregateRoot<Guid>`，Author 为 `FullAuditedAggregateRoot<Guid>`；官方模型的 ConcurrencyStamp 是并发 token。普通 GUID CRUD、并发戳冲突、软删除、单表排序分页和同一事务内两个 DbContext 复用连接/共同回滚已有本仓库八项真实小模型测试，不重复报为缺失能力。

固定 ABP `IdentitySecurityLogStore.SaveAsync` 调用 `Begin(requiresNew: true)`，插入日志并 Complete。账户旅程需要此独立工作单元，不允许关闭日志或把独立事务合并到外层以制造成功。已有共同回滚测试不覆盖独立连接的内层提交。

独立 probe 使用同一数据库目录的两个上下文/连接：外层事务更新用户而未提交，内层事务插入安全日志并提交，随后外层回滚；重新打开上下文后用户变化必须消失，日志必须保留。该合同不同于同一连接开启两个事务或要求数据库支持 SAVEPOINT，也不能未经确认把 ABP 默认事务写成 Serializable。

### 7. 实际账户 HTTP 旅程边界

`tests/SonnetDB.Abp.BookStore.Tests/BookStoreHttpJourneyTests.cs` 使用标准 Migrator 两次、实际官方 Web Program、admin 登录、Cookie、防伪与权限。覆盖 Authors/Books 页面，作者与书籍新增/更新，书籍名称分页，重建宿主后详情读取，删除书籍/作者及作者过滤结果。

该完整旅程目前使用 PostgreSQL。现有普通 filter 不含 LIKE 特殊字符，也没有覆盖作者名排序、全部 Administration 页面或真实 Passkey 身份验证；不能将本次通过推导到这些额外范围。SonnetDB 需待公开包完整模型/标准迁移可执行后，复用同一旅程验收。

## 潜在风险与最小 probe 顺序

以下是源码候选或未覆盖合同，不是新增 issue 的既成事实。SQL 表述为待验证结构；实际 SQL 应从固定公开包 `ToQueryString()` 或命令拦截记录取得。

| 优先级 | 候选 | 最小 probe 与判定 |
|---|---|---|
| 1 | 参数化 Contains/StartsWith 的字面转义 | 当前 SonnetDB 源码 StringMethodTranslator 常量分支转义 `%`、`_`、反斜杠，参数分支直接拼 LIKE 模式。插入 `100% Real`/`100X Real`、`A_B`/`AXB` 等对照名，参数 `%`、`_` 与反斜杠必须按字面匹配；独立比较列表和计数。公开 4.0.0 真实结果失败才确认缺口 |
| 2 | JOIN 投影、排序、int 分页和集合 materialization | 小模型复用官方查询形状，核对详情、书名/作者名分页、总数及重复关联行；只有 SQL 生成或一个简单分页通过不够 |
| 3 | 角色 UNION 与权限集合参数 | 空/单项/多项集合，直授和组织角色重叠时 UNION 去重；权限不得越过 ProviderKey。保存翻译错误、SQL 和实际结果 |
| 4 | BLOB 主键、SequenceEqual/EXISTS 与复合键 | 无 JSON User/Passkey/UserToken 小模型；不同数组实例相同内容须定位同一用户，重复二进制/复合键须拒绝 |
| 5 | requiresNew 独立提交 | 两上下文分别提交/回滚，重新打开后核对用户和日志；不得关闭事务、追加隐式重试或跳过失败 |
| 6 | 标准迁移生命周期与模块索引约束 | 实际生成 SQL、首次迁移、重复迁移、种子复跑分别记录；JSON 闭环前用明确隔离模型，不声称完整 BookStore 模型通过 |

## 查重与 issue 门禁

本轮本地查重回执明确不是穷尽搜索。已记录：DDL 事务与 #227 重叠；主键演进 #226 是升级条件范围；命名外键 #188 已关闭但仍需公开包复测；索引方向/include/约束元数据与 #250 部分重叠。DateTime 亚毫秒参数精度 #236 可按真实审计/日期路径复用。

普通 Book.Price 是 float；decimal 绑定/precision 的 #235/#241/#264 不能自动计入当前 A06 阻塞。字符串函数 #174、其它 JOIN #177 的引擎 issue 状态不等于 EF 翻译/转义/关联分页已验收；Int64 分页 #256、Serializable #225、UPDATE JOIN #246 也不是当前默认业务路径的直接证明。

提交新需求前应同时具备：准确实际消费路径、固定公开包与 EF 版本、独立最小模型/完整可运行代码、实际 SQL 和真实失败、SQLite 同合同对照、失败阶段、查重结果以及公开修复包后的复测条件。只读源码风险和 restore/build 成功不应进入“已复现”栏；相同引擎需求复用旧 issue，只有独立 Provider 缺口或包回归才另报。

## 来源与执行归属

应用来源为本仓库固定官方 BookStore；内置模块分析参照 ABP `10.7.0` 标签的以下文件：

- [IdentityDataSeeder](https://github.com/abpframework/abp/blob/10.7.0/modules/identity/src/Volo.Abp.Identity.Domain/Volo/Abp/Identity/IdentityDataSeeder.cs)
- [IdentityUserStore](https://github.com/abpframework/abp/blob/10.7.0/modules/identity/src/Volo.Abp.Identity.Domain/Volo/Abp/Identity/IdentityUserStore.cs)
- [EfCoreIdentityUserRepository](https://github.com/abpframework/abp/blob/10.7.0/modules/identity/src/Volo.Abp.Identity.EntityFrameworkCore/Volo/Abp/Identity/EntityFrameworkCore/EfCoreIdentityUserRepository.cs)
- [IdentityEfCoreQueryableExtensions](https://github.com/abpframework/abp/blob/10.7.0/modules/identity/src/Volo.Abp.Identity.EntityFrameworkCore/Volo/Abp/Identity/EntityFrameworkCore/IdentityEfCoreQueryableExtensions.cs)
- [EfCorePermissionGrantRepository](https://github.com/abpframework/abp/blob/10.7.0/modules/permission-management/src/Volo.Abp.PermissionManagement.EntityFrameworkCore/Volo/Abp/PermissionManagement/EntityFrameworkCore/EfCorePermissionGrantRepository.cs)
- [EfCoreOrganizationUnitRepository](https://github.com/abpframework/abp/blob/10.7.0/modules/identity/src/Volo.Abp.Identity.EntityFrameworkCore/Volo/Abp/Identity/EntityFrameworkCore/EfCoreOrganizationUnitRepository.cs)
- [IdentitySecurityLogStore](https://github.com/abpframework/abp/blob/10.7.0/modules/identity/src/Volo.Abp.Identity.Domain/Volo/Abp/Identity/IdentitySecurityLogStore.cs)

SonnetDB `D:/source/SonnetDB` 工作树源码仅作为候选风险来源，不代表 NuGet 4.0.0 的实际行为，且本分析未修改该工作树。本页整理只读取已有分析回执和来源，不新增网络查询、构建、测试、数据库/服务或 issue POST；仅新增本文件，无额外进程、下载或临时资源需要回收。
