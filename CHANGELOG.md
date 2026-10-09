# CHANGELOG

## [Unreleased]

### Added

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
