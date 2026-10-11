# ROADMAP

更新：2026-10-11。主线是先把选定的 ABP 官方示例完整纳入本仓库，再按顺序逐个迁移到 SonnetDB，最终完成九种原生模型涉及的组件适配。

第一批固定为 **13 个官方示例**，源码导入 **13/13 ✅**，完整 SonnetDB 迁移验收 **0/13**。BookStore 已完成接线与 PostgreSQL 对照，SonnetDB 完整模型仍受上游阻塞；另外 12 个仅导入源码，尚未迁移、构建或运行。来源均固定为 abp-samples 提交 97bc6ddfbc491843ddcd101342ce10e088c1d0d6。完整目录索引见 [samples/README.md](samples/README.md)。

状态：✅ 已完成并有验证证据；🔄 正在进行；⬜ 待开展；⛔ 已复现并受上游能力缺口阻塞。编号是稳定任务 ID，第一列序号是当前推进顺序，调整顺序不重新编号。计划、编译通过和引擎已有某能力都不能标成适配完成。

## 推进顺序

| 顺序 | 编号 | 状态 | 任务 | 完成条件 |
| --- | --- | --- | --- | --- |
| 01 | A01 | ✅ | 独立仓库与关系型 EF Core 配置桥接 | 默认/指定 DbContext、动态连接串、ExistingConnection 复用；8 项真实本地仓储/UOW/过滤测试、控制台样例与初始化 CI 通过。仅此范围完成，不能代表完整 Web 应用或九模型完成 |
| 02 | A17 | ✅ | 第一批 13 个官方示例清单、来源与完整源码入库 | S01 BookStore 原 195 文件已在仓库；新增 S02–S13 共 1662 原文件，逐文件核对原始 Git blob 与复制 SHA-256，保留 MIT 许可证、来源与迁移记录。只完成导入，不代表构建或兼容 |
| 03 | A06 | ⛔ | S01 BookStore：SQL Server 官方应用迁移到 SonnetDB | 保留完整分层、业务/UI、登录和模块；完整模型受 #266 阻塞；新增 #270 作者筛选、#271 二进制查询；后续升级/脚本部署受 #272/#273 影响。修复后独立验收迁移/种子、账户、CRUD/分页与重启 |
| 04 | A18 | ⬜ | S02 EfCoreJSONColumnDemo 迁移 | 固定依赖后先建立 JSON 列参照；用 SonnetDB 验证 owned/complex JSON 聚合、嵌套/数组/null、查询、更新、迁移和回滚。共享 #266 需求，但本示例未经运行，不能预写已复现或完成 |
| 05 | A19 | ⬜ | S03 EfCoreMigrationDemo 迁移 | 保留官方迁移流程；SonnetDB 首次、重复与增量迁移、种子和数据保留通过；迁移生成与实际执行分别验收 |
| 06 | A20 | ⬜ | S04 MultipleDbContext 迁移 | 完整示例多 DbContext 的仓储、同库连接/事务复用、各迁移和共同回滚；跨库边界单独说明，不从 A01 的小型测试推导 |
| 07 | A21 | ⬜ | S05 PostgeSqlDemo 迁移 | 保留官方 PostgreSQL 示例的业务合同，完成参照与 SonnetDB 对照；类型、查询、迁移、种子和应用操作逐项验证；保留官方目录拼写 |
| 08 | A22 | ⬜ | S06 UnifiedEfCoreMigrations 迁移 | 官方统一 DbContext/迁移方式使用 SonnetDB 后，模块模型、首次/增量迁移、种子和实际读写通过 |
| 09 | A23 | ⬜ | S07 Dapper 迁移 | 通过公开 SonnetDB ADO.NET 能力与 ABP Dapper 扩展接线；参数、查询、分页、受影响行数、事务复用和回滚；通用 SQL 缺口回 SonnetDB |
| 10 | A02 | ⬜ | 完整 Demo 的迁移、关系模型与查询合同汇总 | 实际模型的首次/重复/升级迁移，索引、唯一/外键约束、JOIN、Include、排序分页、decimal/时间/Guid/null、并发冲突与回滚；不以 EnsureCreated 替代迁移验收 |
| 11 | A04 | ⬜ | ABP 内置关系模块逐项验收 | Identity 用户/角色/组织、Account 登录注销、Permission/Setting/Feature Management、Tenant Management、Audit Logging、Background Jobs、OpenIddict 各自完成真实存储与 HTTP/UI 旅程；不删除模块或禁用过滤绕开失败 |
| 12 | A03 | ⬜ | 真实 Server、连接管理与租户隔离 | 同一 Demo 对接实际 SonnetDB Server；配置切换、超时取消、连接复用、多 DbContext、每租户独立数据库；宿主/两租户及各模型权限边界分别验证 |
| 13 | A24 | ⬜ | S08 BookStore-Angular-MongoDb 迁移 | 保留 Angular UI 与书籍业务，通过 SonnetDB 文档适配完成仓储/应用服务、种子、登录、CRUD、查询与分页；不假设 MongoDB Driver/wire 兼容 |
| 14 | A08 | ⬜ | JSON 文档与应用服务/仓储组件验收 | 从 S08 的实际需求完善公开文档 SDK 与必要 Repository/UOW 扩展；JSON 使用 source-generated System.Text.Json；索引、审计、软删除、租户、失败取消分别验收 |
| 15 | A25 | ⬜ | S09 RabbitMqEventBus 迁移 | 保留官方 App1/App2 与事件合同，通过 SonnetMQ 的 ABP 事件适配完成发布/订阅、跨进程传递与失败重试；不假设 RabbitMQ 协议兼容 |
| 16 | A26 | ⬜ | S10 KafkaEventBus 迁移 | 保留官方生产/消费业务，验证 SonnetMQ 下消费、顺序、重启与 replay；明确与 Kafka 的语义边界 |
| 17 | A27 | ⬜ | S11 DynamicDistributedEvents 迁移 | 保留共享业务和现有 Provider 对照；接入 SonnetMQ 的字符串事件名、动态载荷和多进程发布/订阅，并分别验证各入口 |
| 18 | A28 | ⬜ | S12 ModularCrm-OutboxInbox-Pattern 迁移 | 保留 CRM 模块、关系写入与官方 outbox/inbox；SonnetDB + SonnetMQ 验证事务提交/回滚、投递重试、幂等与重启补偿 |
| 19 | A10 | ⬜ | SonnetMQ 与分布式事件组件验收 | 汇总 S09–S12：ACK、重试、幂等、顺序、replay 与 outbox/inbox；明确实例级 MQ 与单库备份边界，不承诺 exactly-once |
| 20 | A29 | ⬜ | S13 SeparateBackgroundJob 迁移 | 保留独立作业执行器与应用进程；SonnetDB 存储下任务入队、跨进程执行、失败重试、重启恢复和重复执行边界验收 |
| 21 | A11 | ⬜ | 后台任务、锁与组件协作 | 结合 S13 验证 ABP Background Jobs/Workers、缓存与事件协作；若使用 KV/MQ 锁或任务后端，单独验收互斥、租约过期、重试、取消和重复执行 |
| 22 | A07 | ⬜ | KV 与 ABP 分布式缓存适配 | 在 BookStore 增加可操作缓存示例；公开 SonnetDB.Caching.Distributed 接入 IDistributedCache 与 ABP 类型化缓存，验证 TTL、滑动过期、批量、租户/键前缀、清除、重启和远程模式 |
| 23 | A09 | ⬜ | 对象存储与 ABP BLOB Storing 适配 | 在 BookStore 增加附件示例，实现 BlobProvider 并保留容器/租户规范；上传/下载/覆盖/存在判断/删除、大对象取消与重启完整性；Range/Multipart 另验 |
| 24 | A12 | ⬜ | 时序模型接入 | 同一 Demo 增加指标写入/查询应用服务，以公开时序 SDK 接入 DI、权限、租户与取消；批量、时间范围/窗口聚合、Retention、重启读取；明确与关系 UOW 边界 |
| 25 | A13 | ⬜ | 全文检索接入 | 书籍/文档搜索应用服务使用公开全文 API；索引更新/删除、分词、分页、租户权限与重建；明确关系写入到索引的一致性与补偿 |
| 26 | A14 | ⬜ | 向量检索接入 | 公开 SonnetDB / Microsoft.Extensions.VectorData 组件接入应用服务；写入、过滤、Top-K、删除、维度错误与重启；真实 embedding 质量另验，合成向量只证明代码路径 |
| 27 | A15 | ⬜ | 原生属性图接入 | 公开图 API/SQL 接入图应用服务；顶点/边 CRUD、遍历、结果预算、租户权限与重启；保留上游 Graph Beta 标签，不扩大为生产稳定支持 |
| 28 | A16 | ⬜ | 13 示例与九模型完整应用联合验收 | 13 个官方例子逐项完成 SonnetDB 旅程，九模型均有可操作入口与独立结果；混合请求、权限/租户、错误传播、取消、持久化、恢复和资源回收；本地/Server 分别报告 |
| 29 | A05 | ⬜ | 上游闭环与适配包交付 | 能力缺口随各步骤立即提交 SonnetDB issue 并复测；核对支持矩阵、版本、文档与 CI。许可证、NuGet 发布及必要 ABP PR 由维护者确定，未经明确请求不发布 |

本次继续 A06，按模型、HTTP 旅程和 CI 三个互不重叠的范围使用子智能体审查与实现，主智能体集中验证。2026-10-11 回读 NuGet 后，公开 SonnetDB.EntityFrameworkCore 仍为 4.0.0，#266 仍 open；同包重新执行纯 Provider 复现和完整官方迁移生成，仍在 JSON 容器存储类型处失败。新增实际 Passkey 的 Owned JSON 模型复现与有界复测入口，详见 [A06 复测记录](docs/issues/A06-ef-json-columns.md)。原 main 的 GitHub CI 37922480045 已回读为 success，仅覆盖既有基线，不代表完整 SonnetDB 应用通过。

A17 源码入库已完成。当前首个迁移任务仍为 A06；A01 为历史完成项，原 A01–A16 ID 均保留。后续 S02–S13 各有 A18–A29 迁移任务，按顺序推进；受阻时记录 issue，可继续不依赖该缺口的下一项。每次实现归属明确任务 ID，同步 CHANGELOG、HANDOFF 和验证记录。

## 第一批示例逐项进度

导入列只表示源码已在本仓库；迁移列表示 SonnetDB 的实际进展。共 13 项，不将一个例子的多个进程或项目重复计数。

| 顺序 | 示例 | 本仓库目录 | 原文件数 | 源码导入 | SonnetDB 迁移 | 任务 |
| --- | --- | --- | --- | --- | --- | --- |
| S01 | BookStore-Mvc-EfCore | [samples/BookStore](samples/BookStore/DEMO.md) | 195 | ✅ | ⛔ 已接线，完整模型受 #266 阻塞 | A06 |
| S02 | EfCoreJSONColumnDemo | [源码与记录](samples/official/EfCoreJSONColumnDemo/ADAPTATION.md) | 142 | ✅ | ⬜ | A18 |
| S03 | EfCoreMigrationDemo | [源码与记录](samples/official/EfCoreMigrationDemo/ADAPTATION.md) | 135 | ✅ | ⬜ | A19 |
| S04 | MultipleDbContext | [源码与记录](samples/official/MultipleDbContext/ADAPTATION.md) | 254 | ✅ | ⬜ | A20 |
| S05 | PostgeSqlDemo | [源码与记录](samples/official/PostgeSqlDemo/ADAPTATION.md) | 121 | ✅ | ⬜ | A21 |
| S06 | UnifiedEfCoreMigrations | [源码与记录](samples/official/UnifiedEfCoreMigrations/ADAPTATION.md) | 134 | ✅ | ⬜ | A22 |
| S07 | Dapper | [源码与记录](samples/official/Dapper/ADAPTATION.md) | 66 | ✅ | ⬜ | A23 |
| S08 | BookStore-Angular-MongoDb | [源码与记录](samples/official/BookStore-Angular-MongoDb/ADAPTATION.md) | 216 | ✅ | ⬜ | A24 |
| S09 | RabbitMqEventBus | [源码与记录](samples/official/RabbitMqEventBus/ADAPTATION.md) | 19 | ✅ | ⬜ | A25 |
| S10 | KafkaEventBus | [源码与记录](samples/official/KafkaEventBus/ADAPTATION.md) | 19 | ✅ | ⬜ | A26 |
| S11 | DynamicDistributedEvents | [源码与记录](samples/official/DynamicDistributedEvents/ADAPTATION.md) | 30 | ✅ | ⬜ | A27 |
| S12 | ModularCrm-OutboxInbox-Pattern | [源码与记录](samples/official/ModularCrm-OutboxInbox-Pattern/ADAPTATION.md) | 379 | ✅ | ⬜ | A28 |
| S13 | SeparateBackgroundJob | [源码与记录](samples/official/SeparateBackgroundJob/ADAPTATION.md) | 147 | ✅ | ⬜ | A29 |

共 1857 个上游原文件：BookStore 的 195 个已导入并作必要适配；本次新增 12 项的 1662 个文件保持原始字节，附各自许可证、Git blob/SHA-256 清单与 ADAPTATION.md。原始项目保留上游版本与说明，进入各自迁移任务时才集中固定依赖、接入适配包、加入根解决方案和 CI，不能把尚未运行的原始示例计为检查通过。

首批 13 个是官方示例迁移数量；KV、BLOB、时序、全文、向量和图仍按 A07/A09/A12–A15 在 BookStore 增加独立场景，不能用关系型示例代替验收。后续新增官方例子需先更新清单、总数和来源，再入库、迁移。

A06 当前子项：✅ 官方完整示例导入（195 文件与 MIT 来源）、集中依赖/Provider 接线、Release 编译、PostgreSQL 标准迁移/种子与真实账户 HTTP CRUD/分页/宿主重启读取；✅ SQLite 标准迁移与局部业务对照。提交前已重新完成本地 restore、格式检查、Release 构建和测试，零警告/错误、20/20 测试无跳过；新增远端 CI 以对应提交的实际 run 另验。SQLite 完整登录遇到安全日志独立事务与数据库写锁冲突，原失败保留，未通过关闭事务/安全日志处理，完整基线改用 PostgreSQL。⛔ SonnetDB 官方模型 JSON 列映射，已提交并回读 [SonnetDB #266](https://github.com/IoTSharp/SonnetDB/issues/266)。[最小复现与验收合同](docs/issues/A06-ef-json-columns.md) 已纳入仓库；纯 Provider 程序也复现相同失败。A06 仍未完成，可独立步骤按下一任务 ID 推进。

2026-10-11 A06 本轮新增 Owned JSON 复现和有界复测脚本；SQLite 两种 JSON 模型均通过，SonnetDB 两种模型与完整官方模型仍失败，入口明确退出 1。首次既有回归 19/20 的 SQLite 夹具函数注册锁错误保留，改为各 DbContext 独立连接后，最终 restore/format/Release build/test 全部通过，0 警告/0 错误、20/20 无跳过。该测试夹具修复不改变历史完整 SQLite 登录的验收状态，也不解除 SonnetDB #266。

同日继续分析 JSON 之外的缺口，公开 4.0.0 的 18 个非 JSON 场景中 11 PASS/7 FAIL；失败归并为四项并独立 GET 回读：[作者参数 Contains #270](https://github.com/IoTSharp/SonnetDB/issues/270)、[BLOB/Passkey 参数查询 #271](https://github.com/IoTSharp/SonnetDB/issues/271)、[EF 外键名丢失与无效 DROP #272](https://github.com/IoTSharp/SonnetDB/issues/272)、[Idempotent 脚本重放 #273](https://github.com/IoTSharp/SonnetDB/issues/273)。后两项属于后续升级/脚本部署，不宣称初次建库都因此失败；DDL 事务复用现有 #227，并发迁移锁与完整恢复仍未验。四份 issue 的完整独立代码另行构建并运行；根解决方案最终门禁仍为 0 警告/0 错误、20/20。详见 [A06 缺口矩阵和回执](docs/issues/A06-sonnetdb-gap-analysis.md)，不将独立失败 probe 计入通过测试。

## 官方示例与改动约束

- 主基线选 [BookStore-Mvc-EfCore](https://github.com/abpframework/abp-samples/tree/97bc6ddfbc491843ddcd101342ce10e088c1d0d6/BookStore-Mvc-EfCore)：完整分层 MVC/Razor Pages、书籍/作者业务、Identity、权限、设置、租户、审计与 OpenIddict。上游原用 SQL Server，本仓库使用 PostgreSQL 完整基线，SQLite 局部对照，再切 SonnetDB。
- 第一批 13 个已先导入 samples/，固定来源提交并保留第三方许可证；随后逐个迁移，逐项记录必要改动。原始源码入库与可运行 SonnetDB 适配是两个验收阶段。
- 保留示例业务/UI；改动集中于依赖、Provider 注册、连接配置和必要迁移接线。仅用本仓库适配包和公开固定版本 NuGet，不跨仓库引用、复制引擎或修改缓存。
- ABP 框架保持上游版本。本仓库处理 DI、配置、连接复用与 ABP 接口适配；SonnetDB 处理 SQL、事务、类型、存储和通用 EF Provider。确认 ABP 自身缺陷且现有扩展点不足后，才整理最小复现与必要 PR。
- ABP 没有标准接口的时序/全文/向量/图通过应用服务和公开 SDK 接入，不伪造框架 Provider，不让 EF Core 假装覆盖全部模型。

## 九模型覆盖账本

依据 [SonnetDB README（固定提交）](https://github.com/IoTSharp/SonnetDB/blob/5424b66672e37f49a2a5633fdde0b54a9b1f0290/README.md)。这里记录 ABP 适配状态，不是引擎成熟度；各模型的完整适配均尚未验收。

| 原生模型 | ABP 接入点 | 公开组件候选 | 归属 | 当前状态 |
| --- | --- | --- | --- | --- |
| 关系表 | EF Core、Repository、UOW、内置模块、Dapper | SonnetDB.EntityFrameworkCore + 本仓库适配层与公开 ADO.NET | A01/A02/A04/A06/A18–A23 | 🔄 基础 8 项通过；⛔ SonnetDB 完整 Demo 受 #266 阻塞 |
| KV | IDistributedCache、类型化缓存；锁另验 | SonnetDB.Caching.Distributed、SonnetDB 的 KV SDK | A07/A11 | ⬜ |
| JSON 文档 | 应用服务、必要仓储扩展 | SonnetDB 的文档 SDK | A08/A24 | ⬜ |
| 对象存储 | ABP BLOB Storing | SonnetDB 的对象 SDK | A09 | ⬜ |
| 消息队列 | Distributed Event Bus、outbox/inbox、后台任务协作 | SonnetDB 的 MQ SDK | A10/A11/A25–A29 | ⬜ |
| 时序 | 指标应用服务 | SonnetDB 的时序 SDK | A12 | ⬜ |
| 全文检索 | 搜索应用服务 | SonnetDB 的全文 SDK | A13 | ⬜ |
| 向量检索 | 检索应用服务、VectorData 抽象 | SonnetDB / Microsoft.Extensions.VectorData 适配 | A14 | ⬜ |
| 原生属性图 | 图应用服务 | SonnetDB 公开图 API/SQL，Graph Beta | A15 | ⬜ |

开始前核对候选 API 是否已存在于固定公开包；源码已有但包未发布的能力仍是阻塞，不能依赖本地引擎源码制造通过。跨模型原子性不得从关系 UOW 推导。

## 能力缺口与 issue 合同

SQLite/MySQL/PostgreSQL 已提供而 SonnetDB 未提供、且完整 Demo 确实依赖的能力，先最小复现并查重。适配错误在本仓库修复；引擎/通用 Provider 缺口向 [IoTSharp/SonnetDB issues](https://github.com/IoTSharp/SonnetDB/issues) 提交或补充。

每个 issue 包含：任务 ID 与受阻旅程、固定版本、可执行复现与模型/SQL/参数、参照 Provider 的预期及实际结果、SonnetDB 异常/堆栈、所需语义与验收条件、本地/Server 边界和脱敏证据。链接写回路线图/HANDOFF；受阻子项标 ⛔，可独立步骤继续。上游修复后固定新公开包并重跑同一复现与完整 Demo，issue 关闭不代替验收。

## 验证门槛

分别验收还原/编译、建库/迁移、后台模块、HTTP/UI 旅程、重启持久化、Server、恢复与发布。保留原失败，不用其他 Provider 的成功覆盖 SonnetDB 失败。

提交前在最终树执行 CI 的 restore、format verify（severity warn）、Release build/test；修改后重跑受影响检查。日志、bin/obj、数据库和 nupkg 为忽略的本地证据。遵守 PS7、有界执行、身份账本与仅清理本任务资源的要求。
