# 官方示例来源与本仓库改动

- 来源：abpframework/abp-samples 的 BookStore-Mvc-EfCore。
- 固定提交：97bc6ddfbc491843ddcd101342ce10e088c1d0d6（2026-10-09 导入）。
- 上游许可证：UPSTREAM-LICENSE，MIT，Copyright (c) 2020 ABP；仅覆盖来源中的第三方文件，本仓库适配层许可证另由维护者确定。
- 原始 195 个文件完整导入，SQL Server 迁移作为来源记录保留并从编译排除。来源文件清单和原始 SHA-256 见 upstream-files.json。

本仓库必要改动：

1. 依赖改为根 Directory.Packages.props 集中固定；ABP 与现有适配包统一到 10.7.0，EF Core 10.0.12，目标框架统一 net10.0；继承 Nullable、ImplicitUsings、TreatWarningsAsErrors。
2. 使用公开 PostgreSQL/SQLite Provider 和本仓库 SonnetDB.Abp.EntityFrameworkCore，通过 Database:Provider 与 ConnectionStrings:Default 选择；不修改 ABP 框架或 SonnetDB 引擎。PostgreSQL 按 ABP 官方指导开启 Npgsql legacy timestamp 行为，保留示例的时间数据合同。
3. PostgreSQL/SQLite/SonnetDB 各自使用标准 EF migrations 程序集；保留上游 SQL Server 历史文件。设计时与运行时共用 Provider 选择，显式生成迁移，不在运行时自动启动 ABP CLI 或寻找工作区。
4. 因集中开启警告门禁而需要的空值标注及格式化单独计为导入整理，不改变书籍/作者业务合同。
5. Web 前端/服务端统一官方 LeptonX Lite 5.7.0（ABP 10.7.0），沿用资源映射与 npm lock；node_modules/wwwroot/libs、日志和数据库为本地产物。
6. 官方 README 顶部补充本仓库 SonnetDB 适配入口、真实阻塞与第一批 13 个示例索引，下文官方原说明保留；来源哈希仍记录上游原文件，不冒称改动后字节不变。
7. A06 复测中，SQLite 测试夹具的多个 DbContext 复用同一连接对象，种子初始化的函数注册遇到活动语句而失败。改为 GUID 命名的共享内存库，保留 keeper 维持数据库生命周期，各 DbContext 使用独立连接；创建失败释放 keeper。仅调整测试资源隔离，不改业务、授权、事务或安全日志。首个失败保留；这与完整 SQLite 登录的历史写锁是不同验证路径。

上游 README、解决方案、Studio 元数据和脚本保留为来源记录；本仓库运行方式以 DEMO.md 为准。导入或构建不代表 SQLite/SonnetDB 完整旅程通过，实际结果写入 DEMO.md、根 HANDOFF 和 ROADMAP 的 A06。
