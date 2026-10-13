# 示例清单与迁移进度

第一批 **13 个官方示例**，已全部纳入当前仓库；先完成源码导入，再按 S01–S13 顺序迁移 SonnetDB。总推进顺序与验收条件见 [ROADMAP](../ROADMAP.md)。

- ✅ 源码导入：13/13；上游原文件共 1857 个。
- 🔄 S01 已固定公开 JSON 修复包，标准初始迁移、账户 HTTP 与 Passkey 存储回归通过；全部模块、升级与恢复仍独立验收，PostgreSQL 对照保留。
- ⬜ S02–S13：源码已导入，SonnetDB 迁移尚未开展。
- 完整 SonnetDB 示例验收：0/13。构建或另一 Provider 通过不计为 SonnetDB 完成。

| 顺序 | 官方例子 | 目标与迁移入口 | 导入 | SonnetDB 迁移 | 任务 |
| --- | --- | --- | --- | --- | --- |
| S01 | BookStore-Mvc-EfCore | [完整 MVC/EF 书店、Identity 与模块](BookStore/DEMO.md) | ✅ | 🔄 JSON 与初始应用路径通过 | A06 |
| S02 | EfCoreJSONColumnDemo | [JSON 列](official/EfCoreJSONColumnDemo/ADAPTATION.md) | ✅ | ⬜ | A18 |
| S03 | EfCoreMigrationDemo | [标准数据库迁移](official/EfCoreMigrationDemo/ADAPTATION.md) | ✅ | ⬜ | A19 |
| S04 | MultipleDbContext | [多 DbContext](official/MultipleDbContext/ADAPTATION.md) | ✅ | ⬜ | A20 |
| S05 | PostgeSqlDemo | [PostgreSQL 数据类型和行为对照](official/PostgeSqlDemo/ADAPTATION.md) | ✅ | ⬜ | A21 |
| S06 | UnifiedEfCoreMigrations | [统一 DbContext/迁移](official/UnifiedEfCoreMigrations/ADAPTATION.md) | ✅ | ⬜ | A22 |
| S07 | Dapper | [ADO.NET/Dapper 与 UOW](official/Dapper/ADAPTATION.md) | ✅ | ⬜ | A23 |
| S08 | BookStore-Angular-MongoDb | [Angular 与文档存储](official/BookStore-Angular-MongoDb/ADAPTATION.md) | ✅ | ⬜ | A24 |
| S09 | RabbitMqEventBus | [分布式事件](official/RabbitMqEventBus/ADAPTATION.md) | ✅ | ⬜ | A25 |
| S10 | KafkaEventBus | [生产/消费与事件顺序](official/KafkaEventBus/ADAPTATION.md) | ✅ | ⬜ | A26 |
| S11 | DynamicDistributedEvents | [动态事件与 Provider 对照](official/DynamicDistributedEvents/ADAPTATION.md) | ✅ | ⬜ | A27 |
| S12 | ModularCrm-OutboxInbox-Pattern | [模块化 CRM 与 outbox/inbox](official/ModularCrm-OutboxInbox-Pattern/ADAPTATION.md) | ✅ | ⬜ | A28 |
| S13 | SeparateBackgroundJob | [独立后台任务执行器](official/SeparateBackgroundJob/ADAPTATION.md) | ✅ | ⬜ | A29 |

全部官方来源固定为 abp-samples 提交 97bc6ddfbc491843ddcd101342ce10e088c1d0d6。BookStore 已作必要接线改动；official/ 下新增 12 个例子的 1662 个原文件逐个核对原始 Git blob 与复制 SHA-256，保留上游源码、README、包版本与 MIT 许可证。每例子的 ADAPTATION.md、upstream-files.json 和 UPSTREAM-LICENSE 记录迁移任务与来源。

新增 12 个原始例子尚未加入根 SonnetDB.Abp.slnx，不计入已通过的 20 项测试。迁移时才统一到根 Directory.Packages.props 的固定依赖，尽量保留业务/UI，以本仓库接口适配替换数据库与组件；通用缺口向 SonnetDB 发 issue，逐个完成建库/种子、真实业务、错误/回滚和重启验证后再更新 ✅。

KV、BLOB、时序、全文、向量和图仍需按路线图在 BookStore 中增加独立可操作场景；13 个官方例子的导入不会代替九模型验收。

本仓库另有 [ABP 仓储控制台样例](SonnetDB.Abp.Sample/) 与 [JSON 列最小复现](SonnetDB.Abp.JsonColumnRepro/)，它们是本项目自有样例/诊断程序，不计入 13 个官方例子。
