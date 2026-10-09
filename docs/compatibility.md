# 兼容性记录

日期：2026-10-09，Asia/Shanghai。

固定依赖：ABP 10.7.0、EF Core 10.0.12、SonnetDB.EntityFrameworkCore 4.0.0、.NET 10。

本地环境：Windows、PowerShell 7.6.6、.NET SDK 10.0.401。数据库使用公开 NuGet 包，不使用 SonnetDB 主仓库源码。

| 检查 | 实际结果 | 本地证据 |
| --- | --- | --- |
| 三项目 Release 构建 | 0 警告、0 错误 | artifacts/checks/build-autofac.json |
| 真实本地集成测试 | 8/8，通过且无跳过 | artifacts/test-results/integration-initial.trx |
| 控制台样例 | 标准 ABP 仓储写入后读取成功 | artifacts/checks/sample-smoke.json |
| 最终完整 restore | 退出 0 | artifacts/checks/restore-final.json |
| 最终 CI 同级 Format Check | 退出 0，verify-no-changes/severity warn | artifacts/checks/format-final.json |
| 最终 Release 构建 | 退出 0，0 警告/0 错误 | artifacts/checks/build-final.json |
| 最终完整本地集成测试 | 8/8，无跳过 | artifacts/test-results/integration-final.trx |
| GitHub Ubuntu CI，初始化代码提交 fb22306 | success | actions/runs/37906959016；已通过独立 API 回读 |

八项覆盖标准 IRepository CRUD/分页、审计时间、显式回滚与未完成工作单元、软删除过滤启停、宿主/两租户隔离、并发戳冲突，以及同库两个 DbContext 的同一连接/同一底层事务与两次写入共同回滚。测试使用 ABP 标准 Autofac 宿主、默认仓储和真实本地数据库；没有替换这些能力的 mock。

首轮构建发现遗漏 Modeling 命名空间，首轮微试发现默认 DI 容器缺少 DbContext 属性注入；补上 ABP 标准 Autofac 宿主后完成微试和全部八项。失败记录保留，不将旧失败覆盖为通过。

最终 restore、Format Check、构建和测试分别保存在 artifacts/checks/*-final.json；这些本地证据未提交。测试数据库带专属 GUID/所有权标记，释放宿主后校验绝对路径和标记再删除。工程命令记录 PID/创建时间/命令行/父进程，只回收可核验归属的进程；不宣称所有未观察后代都已证明不存在。

未验：标准 migrations、远程 SonnetDB Server、每租户独立数据库、ABP Identity/Permission/Setting/OpenIddict、恢复/压力/长稳、Native AOT、包发布。上述 CI success 只覆盖所列提交和 CI 工作流，不扩展为这些未验能力。
