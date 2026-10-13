# CHANGELOG

## [Unreleased]

### Added

- A06 / SonnetDB #266：固定公开 `SonnetDB.EntityFrameworkCore 4.5.0-preview.ef266.5`，补入官方完整模型的标准 SonnetDB 初始迁移与快照，增加账户 HTTP 和真实 IdentityUserStore Passkey 存储回归。公开包内容与已批准 CI 原包匹配，JSON 模型/迁移与实际应用验收分列；A06 全部模块、其它问题和恢复仍独立推进。

- A06：继续分析 JSON 以外的公开 4.0.0 缺口，18 个非 JSON 场景 11 PASS/7 FAIL；查重后创建并 GET 回读 SonnetDB #270（参数化 Contains 字面语义）、#271（BLOB/Passkey 参数查询）、#272（外键名称/无效 DROP）、#273（幂等脚本重放）。DDL 事务复用 #227，迁移锁/完整恢复仍未验。
- A06：新增独立合同诊断项目与有界执行入口，纳入根解决方案；四份 issue 的完整独立代码均另行构建和运行。最终 restore/format/Release build/test 通过，0 警告/0 错误、20/20；已证失败探测单独保留，A06 继续受阻。
- A06：2026-10-11 继续迁移并分工审查模型、HTTP 旅程和 CI；公开 Provider 仍为 4.0.0，#266 仍 open，完整官方迁移生成再次复现原 JSON 映射失败，未将 PostgreSQL 基线或历史 CI success 计为 SonnetDB 完整应用成功。
- A06：在独立 JSON 复现中补充官方 Passkey 的 Owned JSON 字段、用户外键和字符串数组，保留原 Complex 复现与失败退出码；新增 eng/Test-A06ModelReadiness.ps1，在有界执行与独立证据下对照四项模型及完整官方建表脚本生成，失败继续返回失败。
- A06：首次回归 19/20，保留官方 SQLite 种子初始化时重新注册函数遇到活动语句的锁错误；测试夹具改用 GUID 命名共享内存库与独立 DbContext 连接，保留 keeper 和失败释放路径。具体并发语句来源未捕获；不与历史 SQLite 完整登录安全日志写锁混称。
- A06：上述改动后的最终 CI 同级 restore、format verify、Release build/test 均通过，0 警告/0 错误、20/20 无跳过；完整模型复测入口仍为失败（SQLite 两项成功、SonnetDB 三项失败），A06 保持受阻。测试库清理，Demo 容器恢复停止并保留数据。

- A17：首批官方示例固定为 S01–S13，共 13 项，先完整入库、后逐个迁移；新增 12 项/1662 个原文件，核对 Git blob 与复制 SHA-256，逐项保留 MIT 许可证、来源清单与 ADAPTATION.md。
- A17：增加 samples 索引，路线图调整为 29 个有序任务；导入 13/13，完整 SonnetDB 迁移 0/13，S01 受 #266 阻塞，其余各有独立迁移编号和验收条件。新增原始项目尚未构建/运行，不冒称进入已有 CI 验收。
- A17：用户授权提交推送后重新完成最终树 restore、格式检查、Release 构建和真实数据库测试，0 警告/0 错误、20/20 无跳过；测试库清理，PostgreSQL 容器恢复停止并保留数据，原始来源在暂存区继续按原 Git blob 验证。

- A06：按官方完整 Demo 主线重排路线图，添加顺序号、表情状态、九模型组件覆盖、独立验收与 SonnetDB issue 闭环要求。
- A06：导入固定提交的官方 BookStore MVC/EF Core 完整示例及 MIT 来源许可证；添加 PostgreSQL/SQLite/SonnetDB 配置选择与独立迁移程序集，统一主题与集中版本，保留业务/UI 和模块。
- A06：PostgreSQL 标准迁移、重复初始化、真实账户 HTTP CRUD/分页/宿主重启读取与 Kestrel HTTP 通过；SQLite 迁移/种子和局部业务通过，完整登录写锁失败保留。未关闭事务、授权或安全日志绕开失败。
- A06：提供独立 JSON 列最小复现并提交 SonnetDB 需求 #266，完整 SonnetDB Demo 保持受阻状态；CI 新增 PostgreSQL service、前端资源还原与真实 HTTP 旅程。
- A06：最终 restore、格式检查、Release 构建及测试通过，0 警告/0 错误、20/20 测试无跳过；测试库清理，Demo 容器已停止并保留数据。新 CI 按对应提交单独验收，不以本地结果代替远端验收。
- 建立独立 ABP 适配仓库，固定公开 SonnetDB EF Core Provider 与 ABP/.NET 版本。
- 提供 ABP 默认/指定 DbContext 的 UseSonnetDB 配置、配置上下文的动态连接串及 ExistingConnection 复用。
- 添加基于标准 ABP 仓储、真实本地数据库的集成测试、控制台样例与 CI 门禁。

尚无正式发行。
