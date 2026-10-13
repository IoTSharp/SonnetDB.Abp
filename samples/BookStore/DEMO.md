# 完整 BookStore Demo

对应第一批 13 个官方示例中的 **S01**，迁移任务为 ROADMAP 的 **A06**；完整清单和逐例进度见 [示例索引](../README.md)。这个原 SQL Server 项目已纳入根解决方案，并通过本仓库模块接入 SonnetDB。来源与改动见 UPSTREAM.md，业务、Razor 页面和 ABP 模块沿用官方示例。所有 C# 依赖固定在根 Directory.Packages.props，无引擎源码依赖。

## 当前状态（2026-10-14）

| 检查 | PostgreSQL 完整基线 | SQLite 局部对照 | SonnetDB 4.5.0-preview.ef266.5 |
| --- | --- | --- | --- |
| Release 编译 | ✅ 既有基线 | ✅ 既有基线 | ✅ 零警告/错误 |
| 标准初始迁移生成 | ✅ 独立迁移 | ✅ 独立迁移 | ✅ 完整官方模型，含 Passkey JSON |
| 数据库迁移与种子 | ✅ 实际运行及重复初始化 | ✅ 实际运行 | ✅ 初始及重复迁移/种子 |
| 登录/页面/CRUD/分页/应用宿主重建 | ✅ 真实账户 HTTP 基线 | ⛔ 历史安全日志独立事务写锁保留 | ✅ 公开包与真实应用 TestServer |
| Passkey JSON 存储 | ⬜ 单独验收 | ✅ 历史同模型存储对照 | ✅ 真实 IdentityUserStore 插入/读/更新/重建/删除 |
| Web 应用 Kestrel/浏览器 | ✅ 既有 Kestrel HTTP 基线 | ⬜ | ⬜ 不把 TestServer 等同真实浏览器 |
| 九模型、全部模块、升级与故障恢复 | ⬜ 独立后续验收 | ⬜ | ⬜ 不从初始应用旅程推导 |

根依赖已固定公开修复预览包；标准初始迁移、Designer 和快照位于 SonnetDB.Migrations/Migrations。未删除 Identity/Passkey 或修改 ABP 模块。两项持久回归见 tests/SonnetDB.Abp.BookStore.Tests 的 Sonnet HTTP 与 Passkey 测试。[#266 消费验收与历史失败](../../../docs/issues/A06-ef-json-columns.md)按公开包、运行模式和实际范围记录；A06 其余合同仍独立推进。

2026-10-11 已重新读取公开包版本与 issue：SonnetDB.EntityFrameworkCore 仍为 4.0.0，#266 仍 open；原最小复现和完整 BookStore 初始迁移生成仍在 JSON 容器类型映射阶段退出 1，未执行建表。原 main 的 GitHub CI 37922480045 已确认 success，但其通过范围仍是下文的既有基线。

## PostgreSQL 完整 Demo 运行

需要 .NET 10、Node.js/npm 和 PostgreSQL。本机已创建的 Demo 容器名为 sonnetdb-abp-a06-postgres，端口 127.0.0.1:54381；任务结束停止容器，数据保留。首次在其他机器运行可创建同等本地服务，也可用自有 PostgreSQL 修改连接串。无需全局 ABP CLI。

```powershell
# Windows 必须使用 PowerShell 7。
# 本机恢复本轮已创建的容器：
docker start sonnetdb-abp-a06-postgres
# 其他机器首次创建时改用：
# docker run -d --name sonnetdb-abp-a06-postgres -p 127.0.0.1:54381:5432 -e POSTGRES_USER=sonnetdb_abp -e POSTGRES_PASSWORD=sonnetdb_abp_demo -e POSTGRES_DB=BookStore --memory 512m --cpus 2 postgres:17-alpine
$demoConnection = 'Host=127.0.0.1;Port=54381;Database=BookStore;Username=sonnetdb_abp;Password=sonnetdb_abp_demo'
npm ci --prefix samples/BookStore/src/Acme.BookStore.Web --no-audit --no-fund
node eng/Copy-BookStoreAssets.cjs
dotnet restore SonnetDB.Abp.slnx
dotnet build SonnetDB.Abp.slnx -c Release --no-restore
dotnet run --project samples/BookStore/src/Acme.BookStore.DbMigrator -c Release --no-build --no-restore -- --Database:Provider PostgreSql --ConnectionStrings:Default $demoConnection
dotnet run --project samples/BookStore/src/Acme.BookStore.Web -c Release --no-build --no-restore -- --Database:Provider PostgreSql --ConnectionStrings:Default $demoConnection
```

访问 launchSettings 的 https://localhost:44339。示例开发账户为 admin / 1q2w3E*（官方种子默认值）；用于本地 Demo。通过书籍、作者及 Administration 菜单使用官方功能；各 Administration 子模块的完整验收仍归 A04。标准 PostgreSQL/SQLite 初始迁移已纳入源码，运行 Demo 不需要 EF CLI；没有 HTTPS 开发证书时按 .NET 官方开发证书流程准备本机环境。

智能体执行长命令使用 eng/Invoke-CheckedCommand.ps1 的数量/超时和进程身份记录；以上命令是用户交互运行说明，Web 前台进程由用户 Ctrl+C 结束。node_modules、wwwroot/libs、日志、数据库与证据均忽略，不提交。

完整 HTTP 测试需要 PostgreSQL 的创建/删除测试库权限，每次使用专属 GUID 数据库，释放宿主后按所有权校验删除。默认连接上述本机容器；其他环境设置 BOOKSTORE_POSTGRES_CONNECTION。CI 已配置独立 PostgreSQL service 与此变量，真实数据库行为不 mock。

SQLite 可通过 Database:Provider=Sqlite 和一个绝对文件连接串验证局部模型/业务。完整登录原失败 a06-http-test-1 保留：Identity 登录修改用户状态后，安全日志 requiresNew 事务写入遇到 database is locked。没有关闭事务、安全日志或数据过滤来制造完整成功；因此完整对照改用 PostgreSQL。

## 运行 SonnetDB

应用 EF 模块依赖本仓库 SonnetDbAbpEntityFrameworkCoreModule，配置使用 Database:Provider=SonnetDB、ConnectionStrings:Default=Data Source=一个独立目录。Provider 选择只影响数据库接线，不替换标准仓储、Identity 或授权。

在仓库根目录执行下面的目标数据库入口；显式使用绝对目录，确保 Migrator 和 Web 对接同一个库。固定修复包和标准迁移已纳入仓库，前端资源与构建准备沿用上文命令。

```powershell
$demoSonnetDirectory = [IO.Path]::GetFullPath((Join-Path (Get-Location) 'artifacts/bookstore-sonnetdb'))
$demoSonnetConnection = 'Data Source=' + $demoSonnetDirectory
dotnet run --project samples/BookStore/src/Acme.BookStore.DbMigrator -c Release --no-build --no-restore -- --Database:Provider SonnetDB --ConnectionStrings:Default $demoSonnetConnection
dotnet run --project samples/BookStore/src/Acme.BookStore.Web -c Release --no-build --no-restore -- --Database:Provider SonnetDB --ConnectionStrings:Default $demoSonnetConnection
```

纯 Provider 最小复现不创建数据库：

```text
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- Sqlite
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- SonnetDB
```

固定公开修复包后，两者均退出0，分别报告 TEXT 和 JSON；追加 Owned 参数也均通过。2026-10-09/11 对公开4.0.0的原失败保留，不改写历史结果。最小模型仅证明设计模型，数据库与应用结果见上表和独立回归。

原复现覆盖 `ComplexProperty(...ToJson)`；实际 Identity Passkey 使用 `OwnsOne(...).ToJson("Data")`。新增 `Sqlite Owned` / `SonnetDB Owned` 模式保留实际字段类型、用户外键和字符串数组，仍只构造模型，不打开数据库。说明见 [JSON 复现](../SonnetDB.Abp.JsonColumnRepro/README.md)。

完成根解决方案的 restore 与 Release build 后，可在仓库根目录用已安装的 EF CLI 执行统一复测：

```powershell
& 'C:\Program Files\PowerShell\7\pwsh.exe' -File ./eng/Test-A06ModelReadiness.ps1 -Micro
& 'C:\Program Files\PowerShell\7\pwsh.exe' -File ./eng/Test-A06ModelReadiness.ps1
```

微试只执行 SQLite Complex；完整入口依次执行四项 Provider/模型组合及官方完整模型的 `dbcontext script`，每次以唯一编号保存 artifacts/checks 下的日志与汇总。任何失败都使完整入口失败；即使全部通过，也只证明设计时模型与建表 SQL 生成，仍须标准迁移生成、实际执行/种子、Passkey 读写、完整 HTTP 旅程与重启持久化逐项验收。

## 自动验证范围

2026-10-14：只使用 NuGet.org 与全新包缓存还原固定修复包；公共包有效载荷与获批 CI 原包逐项相同，仓库签名验证通过。统一模型入口5/5通过，标准初始迁移由原 EF CLI 生成；新 Sonnet HTTP 和 Passkey 回归各1/1通过。最终完整提交门禁与远端 CI 按新提交的实际结果核验，不复用下列旧20项结果。数据层 REST/HTTP2、完整恢复及应用浏览器验收分别记录，不互相代替。

2026-10-11 本轮修改后的最终树再次完成 restore、format verify、Release build/test，0 警告/0 错误、20/20 无跳过。首次回归 19/20 的 SQLite 种子函数注册锁错误保留；测试夹具改用 GUID 命名共享内存库与独立 DbContext 连接后通过，具体并发语句来源未捕获。此夹具修复未改变历史完整 SQLite 登录安全日志的失败状态。证据为 artifacts/checks/a06-resume-20261011-final-{restore,format,build,test}.json、a06-resume-20261011-final-test-totals.json 与 artifacts/test-results/a06-resume-20261011-final/。独立模型复测仍是 SQLite 两项成功、SonnetDB 三项失败，入口退出 1，A06 仍受阻。

2026-10-09 提交前重新执行最终 CI 同级 restore、格式检查、Release 构建与测试，全部通过，构建零警告/错误，实际 20/20 测试、无失败/跳过：原有 SonnetDB 集成 8 项、官方 SQLite 业务 10 项、官方欢迎页 1 项及 PostgreSQL 完整 HTTP 旅程 1 项。最新证据位于根 artifacts/checks/a17-commit-{restore,format,build,test}.json 与 artifacts/test-results/a17-commit/；均为忽略的本地产物，新 GitHub 工作流依对应提交的实际 run 单独验收。

官方原测试基类的 AlwaysAllowAuthorization 仅用于原业务测试；新增 tests/SonnetDB.Abp.BookStore.Tests 使用实际 Web Program、标准 Identity 登录、Cookie/防伪校验和权限种子，未替换授权或数据过滤。

完整 HTTP 旅程在 ASP.NET TestServer 中运行，验证账户、页面响应、API CRUD、分页以及关闭/重建宿主后的持久化。另有真实 Kestrel 本机监听和四个 HTTP 200 检查（a06-kestrel-http-2.json）；该限时进程在 45 秒窗口后由执行器核验身份并回收，不能计为正常退出。尚无真实浏览器点击、全部 Administration 模块或 SonnetDB Server 验收。任务收尾已独立核对测试库剩余 0、Demo 容器 running=false/PID=0，保留数据供继续运行。
