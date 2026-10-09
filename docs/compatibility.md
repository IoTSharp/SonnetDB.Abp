# 兼容性记录

日期：2026-10-09，Asia/Shanghai。

固定依赖：ABP 10.7.0、EF Core 10.0.12、SonnetDB.EntityFrameworkCore 4.0.0、.NET 10。

本地环境：Windows、PowerShell 7.6.6、.NET SDK 10.0.401。数据库使用公开 NuGet 包，不使用 SonnetDB 主仓库源码。

以下为 A01 初始化阶段的历史验证，不代表新增完整 Demo 已通过。

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

## A06 官方完整 Demo（2026-10-09）

固定来源 abp-samples/BookStore-Mvc-EfCore，提交 97bc6ddfbc491843ddcd101342ce10e088c1d0d6，完整保留 195 个来源文件和 MIT 许可证；必要改动见 samples/BookStore/UPSTREAM.md。根解决方案加入官方项目、PostgreSQL/SQLite/SonnetDB 独立迁移程序集和纯 JSON 列最小复现。

- SQLite 标准迁移生成与实际 Migrator/种子退出 0；对应 a06-sqlite-migration-generate-3、DbMigrator/artifacts/checks/a06-sqlite-migrate-seed-1。
- SonnetDB 官方模型生成迁移退出 1：IdentityUserPasskey.Data JSON 容器缺少 Provider 类型映射。纯 Provider 最小复现 SQLite 退出 0、SonnetDB 退出 1，独立排除了 ABP 接线和旧 EF CLI 作为此异常的必要条件。需求见 [issue 合同](issues/A06-ef-json-columns.md)，已创建并回读 [SonnetDB #266](https://github.com/IoTSharp/SonnetDB/issues/266)。不改 ABP、不移除 Passkey。
- PostgreSQL 同模型初始迁移、实际 Migrator/种子和完整 HTTP 旅程已通过；重复初始化、真实 admin 登录、官方书籍/作者页面、CRUD/分页、重建宿主后读取和删除通过。新增旅程不替换 Identity/权限/防伪机制；原官方业务夹具仍独立保留。TestServer 不等于浏览器点击，另有真实 Kestrel 首页/登录页/静态资源/Swagger 四项 200；45 秒窗口后核验身份回收，非正常退出验收。
- SQLite 完整登录遇到安全日志 requiresNew 事务与数据库写锁冲突（a06-http-test-1）；未关闭事务、安全日志或授权来绕过，改用 PostgreSQL 作完整对照。前端/服务端主题统一官方 5.7.0 后补齐 jquery-migrate 资源。
- 首次集中依赖发现 xUnit execution 版本冲突、空值及 Mapperly 未声明映射合同、Web 测试入口和静态资源 manifest 问题；按导入接线修复并保留首次失败。没有将这些导入问题作为引擎缺口，也没有通过关闭警告或跳过测试处理。

SonnetDB 完整迁移/种子/登录/内置模块及其他八种模型均不能由 SQLite 成功推导。远程 Server、恢复、长稳、Native AOT 和发行策略仍未验；新增 CI 工作流必须按对应提交的实际 run 验收，不能沿用旧提交的 CI success。

A06 原 CI 同级本地门禁如下；远端结果需要单独验收。

| 检查 | 实际结果 | 本地证据 |
| --- | --- | --- |
| 完整 restore | 退出 0 | artifacts/checks/a06-restore-final.json |
| Format Check | 退出 0，verify-no-changes/severity warn | artifacts/checks/a06-format-final.json |
| Release 构建 | 退出 0，0 警告/0 错误 | artifacts/checks/a06-build-final.json |
| 完整测试 | 20/20，无失败/跳过 | artifacts/checks/a06-test-final.json；artifacts/test-results/a06-final/ |
| 真实 Kestrel HTTP | 首页、登录页、静态资源、Swagger 均 200 | artifacts/checks/a06-kestrel-http-2.json |
| 上游 #266 正文回读 | open，独立复现与本地合同正文一致 | artifacts/checks/a06-issue-portable-repro-readback.json |
| 收尾资源核验 | 269 个已记录进程身份匹配存活数 0；测试库 0；容器停止且保留数据 | artifacts/checks/a06-process-final-audit.json；a06-postgres-cleanup.json |

20 项为实际发现计数：SonnetDB 本地集成 8、官方 SQLite 业务 10、官方欢迎页 1、PostgreSQL 完整账户 HTTP 旅程 1。三个支持/基类测试程序集没有独立测试，不额外计数；此结果不覆盖 SonnetDB 完整官方模型的已知失败。

## A17 官方示例先入库（2026-10-09）

第一批固定 13 个官方例子，S01 BookStore 已导入并作接线；S02–S13 的 1662 个原文件本次完整入库，保留原始 Git blob 字节、MIT 许可证、模式/哈希清单和逐例迁移记录。独立回读核验原文件数、Git blob/SHA-256、许可证一致性与 Git 收录范围，隐藏原文件数 0；证据 artifacts/checks/a17-final-source-audit.json。

源码入库阶段只完成导入与路线图/文档，既有根解决方案及编译/测试代码未变化。新增 12 个例子尚未迁移、还原、构建、运行或纳入根解决方案/CI；仍为 ⬜。导入 13/13 与完整 SonnetDB 迁移 0/13 分列记录，不将来源入库推导为兼容，也不计入上面的 20 项实际应用测试。

源码入库阶段未启动数据库或 Web 服务；当时 15 个已记录进程身份最终匹配存活数 0，两个任务 ZIP 和微试目录已清理，来源快照/脚本/日志保留为忽略的本地证据。成功来源核验没有覆盖大小写和换行转换的原失败记录。

用户随后授权提交推送。提交前在最终树重新执行完整 restore、format verify、Release build/test，全部退出 0，构建 0 警告/0 错误，实际 20/20 无失败/跳过；证据 artifacts/checks/a17-commit-{restore,format,build,test}.json、a17-commit-test-totals.json 与 artifacts/test-results/a17-commit/。临时恢复的 PostgreSQL 容器已核验身份并恢复停止，GUID 测试库剩余 0、数据保留；原始来源还在 Git 暂存区核对 Git blob，避免换行过滤改变来源字节。
