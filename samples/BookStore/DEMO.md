# 完整 BookStore Demo

对应第一批 13 个官方示例中的 **S01**，迁移任务为 ROADMAP 的 **A06**；完整清单和逐例进度见 [示例索引](../README.md)。这个原 SQL Server 项目已纳入根解决方案，并通过本仓库模块接入 SonnetDB。来源与改动见 UPSTREAM.md，业务、Razor 页面和 ABP 模块沿用官方示例。所有 C# 依赖固定在根 Directory.Packages.props，无引擎源码依赖。

## 当前状态（2026-10-09）

| 检查 | PostgreSQL 完整基线 | SQLite 局部对照 | SonnetDB 4.0.0 |
| --- | --- | --- | --- |
| Release 编译 | ✅ 零警告/错误 | ✅ | ✅ 编译通过不代表数据库运行 |
| 标准初始迁移生成 | ✅ 独立迁移 | ✅ 独立迁移 | ⛔ IdentityUserPasskey.Data JSON 映射缺失 |
| 数据库迁移与种子 | ✅ 实际运行及重复初始化 | ✅ 实际运行 | ⛔ 先受模型阻塞，未执行 |
| 登录/页面/CRUD/分页/宿主重启 | ✅ 真实账户 HTTP 旅程通过 | ⛔ 登录的安全日志独立事务遇到 SQLite 写锁 | ⛔ 待上游修复后完整重跑 |
| Kestrel 真实监听与 HTTP | ✅ 首页、登录页、静态资源、Swagger 均 200 | ⬜ | ⛔ |
| 九模型、SonnetDB Server 与恢复 | ⬜ 独立后续验收 | ⬜ | ⬜ 不从此基线推导 |

上游需求已提交并回读确认：[SonnetDB #266](https://github.com/IoTSharp/SonnetDB/issues/266)。复现与验收合同见根 docs/issues/A06-ef-json-columns.md；不删除 Identity/Passkey 或改 ABP 来绕过缺口。源码中的 SonnetDB 迁移程序集当前只含标记类型，尚无成功生成的迁移。

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

## 切换 SonnetDB 与重现阻塞

应用 EF 模块依赖本仓库 SonnetDbAbpEntityFrameworkCoreModule，配置使用 Database:Provider=SonnetDB、ConnectionStrings:Default=Data Source=一个独立目录。Provider 选择只影响数据库接线，不替换标准仓储、Identity 或授权。

在仓库根目录执行下面的目标数据库入口；显式使用绝对目录，确保 Migrator 和 Web 对接同一个库。SonnetDB 4.0.0 当前在完整模型的 JSON 列映射处失败，尚不能完成建库/种子或登录；上游修复并生成该 Provider 的标准迁移后，才继续 Web 旅程。前端资源与构建准备沿用上文命令。

```powershell
$demoSonnetDirectory = [IO.Path]::GetFullPath((Join-Path (Get-Location) 'artifacts/bookstore-sonnetdb'))
$demoSonnetConnection = 'Data Source=' + $demoSonnetDirectory
dotnet run --project samples/BookStore/src/Acme.BookStore.DbMigrator -c Release --no-build --no-restore -- --Database:Provider SonnetDB --ConnectionStrings:Default $demoSonnetConnection
# 上游修复、标准迁移生成与 Migrator 成功后继续：
# dotnet run --project samples/BookStore/src/Acme.BookStore.Web -c Release --no-build --no-restore -- --Database:Provider SonnetDB --ConnectionStrings:Default $demoSonnetConnection
```

纯 Provider 最小复现不创建数据库：

```text
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- Sqlite
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- SonnetDB
```

前者退出 0、报告 JSON 列 TEXT；后者退出 1、报告 JSON 容器类型不支持。这两个结果是能力对照，失败没有转换为测试成功。完整标准迁移复现命令见 issue 文档。SonnetDB 公开包修复后，重新生成本 Provider 的迁移，再做实际建库、种子、完整 HTTP/UI 旅程和重启验证。

## 自动验证范围

2026-10-09 提交前重新执行最终 CI 同级 restore、格式检查、Release 构建与测试，全部通过，构建零警告/错误，实际 20/20 测试、无失败/跳过：原有 SonnetDB 集成 8 项、官方 SQLite 业务 10 项、官方欢迎页 1 项及 PostgreSQL 完整 HTTP 旅程 1 项。最新证据位于根 artifacts/checks/a17-commit-{restore,format,build,test}.json 与 artifacts/test-results/a17-commit/；均为忽略的本地产物，新 GitHub 工作流依对应提交的实际 run 单独验收。

官方原测试基类的 AlwaysAllowAuthorization 仅用于原业务测试；新增 tests/SonnetDB.Abp.BookStore.Tests 使用实际 Web Program、标准 Identity 登录、Cookie/防伪校验和权限种子，未替换授权或数据过滤。

完整 HTTP 旅程在 ASP.NET TestServer 中运行，验证账户、页面响应、API CRUD、分页以及关闭/重建宿主后的持久化。另有真实 Kestrel 本机监听和四个 HTTP 200 检查（a06-kestrel-http-2.json）；该限时进程在 45 秒窗口后由执行器核验身份并回收，不能计为正常退出。尚无真实浏览器点击、全部 Administration 模块或 SonnetDB Server 验收。任务收尾已独立核对测试库剩余 0、Demo 容器 running=false/PID=0，保留数据供继续运行。
