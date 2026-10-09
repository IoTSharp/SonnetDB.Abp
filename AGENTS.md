# AGENTS

本仓库是 SonnetDB 的 ABP 集成适配层，独立于 SonnetDB 引擎仓库。

- 开始前阅读根 HANDOFF.md；结束前更新完成项、缺口、验证、Git 状态与资源归属。
- 固定包依赖于 Directory.Packages.props。禁止跨仓库 ProjectReference、复制引擎源码或直接修改 NuGet 缓存。
- ABP 注册、配置、连接复用问题在本仓库修复；通用 SQL、事务与 EF Provider 问题回 SonnetDB 仓库单独处理。
- 不使用 unsafe；Nullable、ImplicitUsings、TreatWarningsAsErrors 必须开启；public API 使用中文 XML 文档。
- 生产 JSON 如有新增，使用 source-generated System.Text.Json。ABP/EF Core 集成不声明 Native AOT 支持，不能冒称核心引擎的 AOT 能力覆盖本包。
- 真实本地数据库测试、远程 Server、ABP 内置模块、恢复与正式发布分别验收；计划或构建不代表完成。
- Windows 只使用 PowerShell 7；执行/搜索/重试/轮询有数量和墙钟上限，先微试，定期报告进度，支持取消。禁用 Graphify、广域工具扫描、猜测编译器、未授权安装与按进程名终止。仅清理已核 PID、创建时间、完整命令行和父链为本任务所有的资源。
- 外网直连失败后使用单次命令范围的 http://127.0.0.1:7890 代理；远程 Bash 用机器规定的 stdin wrapper，不能放入 PowerShell 可展开字符串。
- 单任务对应一个 ROADMAP 编号，更新 CHANGELOG；Conventional Commits。无明确请求不创建子智能体、额外聊天、自动化或发布 NuGet。
- 提交前必须在最终树完成与 CI 相同的检查，修改代码后重新执行受影响检查：

```text
dotnet restore SonnetDB.Abp.slnx
dotnet format SonnetDB.Abp.slnx --verify-no-changes --no-restore --severity warn
dotnet build SonnetDB.Abp.slnx -c Release --no-restore
dotnet test SonnetDB.Abp.slnx -c Release --no-build --no-restore
```

日志、bin/obj、数据库和 nupkg 属于本地验证产物，不得提交。许可证和 NuGet 发布策略需维护者确定。
