# SonnetDB.Abp 交接记录

日期：2026-10-09（Asia/Shanghai）。工作区：D:/source/SonnetDB.Abp。

用户要求以 ABP 官方完整 Demo 为主线，通过本仓库适配 SonnetDB；尽量保留示例，不修改 ABP 框架，通用数据库能力缺口向 SonnetDB 提交 issue。九种原生模型涉及的组件都进入路线图，顺序号与 ✅/🔄/⬜/⛔ 标记分别表达推进顺序和实际验收状态。

本次完成 A17（官方示例清单与源码入库），当前首个迁移任务仍是受阻的 A06。路线图共 29 步，原 A01–A16 编号保留，新增 A17 入库任务与 A18–A29 逐例迁移任务。origin 为公开仓库 https://github.com/IoTSharp/SonnetDB.Abp，分支 main；本轮提交前起点为 d1aceabe14c3791dc9f3b60170464765a1f9a9fd，fetch 已确认当时 origin/main 一致。用户现已明确授权提交与推送 A06/A17 的全部工作，本交接随集成提交保存；最终提交号、远端 SHA 与工作区状态以 git log/status 和 artifacts/checks/a17-publish-state.json 的实际回读为准。SonnetDB 主仓库及其它工作树不属于本任务修改范围。

## 已完成与验证

- A17：按用户要求先入库再逐个迁移。首批选定 13 个官方例子 S01–S13，来源均固定 abp-samples 提交 97bc6ddfbc491843ddcd101342ce10e088c1d0d6；完整清单在 samples/README.md，ROADMAP 同时列入导入与迁移两个状态。
- S01 BookStore 的 195 个来源文件已保留并接线；本次 S02–S13 导入到 samples/official/，共 1662 个原文件，逐文件核对 Git blob 与复制 SHA-256，逐例保留 MIT 来源许可证、文件模式/哈希清单和 ADAPTATION.md。全批共 1857 个原文件，导入 13/13；没有来源文件被 .gitignore 隐藏（a17-final-source-audit.json）。
- 完整 SonnetDB 迁移验收 0/13。S01 已接线但受 #266 阻塞，另外 12 个仍为 ⬜，仅导入，没有还原、构建、运行或纳入根解决方案。上游原 README/包版本保留为来源记录，迁移时再集中固定依赖；没有关闭门禁来宣称原始项目兼容。BookStore README 已加本仓库 SonnetDB 示例入口说明。
- A17 来源入库阶段只新增未迁移源码与文档，根解决方案和既有编译/测试代码未改变；独立验收为原文件数、原始字节/Git blob、许可证、Git 收录范围与 diff check。该阶段没有重跑应用测试；用户随后要求提交推送，已重新执行下列完整门禁，来源入库本身仍不计为应用兼容测试。
- A17 提交门禁：2026-10-09 最终树 restore、format verify（severity warn）、Release build/test 均退出 0；0 警告/0 错误，实际 20/20、无失败/跳过。证据为 artifacts/checks/a17-commit-{restore,format,build,test}.json、a17-commit-gates-summary.json、a17-commit-test-totals.json 与 artifacts/test-results/a17-commit/。已重复制 551 个官方前端资源；新增 12 项原始来源仍不在根解决方案，完整 SonnetDB 迁移仍为 0/13。
- A01 历史完成：独立 EF Core 适配包、ABP/Autofac 控制台样例与八项真实 SonnetDB 仓储/UOW 测试。默认/指定 DbContext、动态连接串、ExistingConnection 复用、共同回滚、审计、软删除、宿主/两租户过滤和并发戳均有证据；样例使用 EnsureCreated，不代表标准迁移完成。
- A06 官方 BookStore-Mvc-EfCore 导入：固定 abp-samples 提交 97bc6ddfbc491843ddcd101342ce10e088c1d0d6，保留 195 个原文件、MIT 来源许可证、Git blob/SHA256 清单及业务/UI。必要接线改动见 samples/BookStore/UPSTREAM.md。
- 依赖集中固定 SonnetDB.EntityFrameworkCore 4.0.0、ABP 10.7.0、EF Core 10.0.12；新增 PostgreSQL/SQLite/SonnetDB 配置选择与独立迁移程序集。无跨仓库 ProjectReference、引擎源码复制或 NuGet 缓存修改；未修改 ABP 框架。
- PostgreSQL 标准初始迁移、实际建库/种子、重复初始化通过。完整 HTTP 旅程使用实际 Web Program、Identity 登录、Cookie/防伪和权限，验证官方作者/书籍页面、CRUD、分页与重建宿主后读取；未替换授权或数据过滤。独立 Kestrel 验证首页、登录页、静态资源与 Swagger 均 HTTP 200；45 秒窗口后核验身份回收，不计为正常退出验收或浏览器点击。
- SQLite 标准迁移/种子与原官方业务对照通过；原官方业务夹具保留 AlwaysAllowAuthorization，不能与新增真实账户 HTTP 旅程混称。前端/服务端主题统一 LeptonX Lite 5.7.0，官方资源映射复制 551 个文件；node_modules 与 wwwroot/libs 忽略。
- 最终 CI 同级 restore、format verify（severity warn）、Release build/test 均退出 0，构建 0 警告/0 错误。实际发现测试 20/20 通过、无失败/跳过：SonnetDB 集成 8、官方 SQLite 业务 10、官方欢迎页 1、PostgreSQL 完整 HTTP 旅程 1。三个支持/基类测试程序集未发现独立测试，不额外计数。
- A06 原门禁证据 artifacts/checks/a06-{restore,format,build,test}-final.json，TRX 为 artifacts/test-results/a06-final/，汇总 a06-final-test-totals.json；这些均为忽略的本地证据。新工作流已增加前端资源和 PostgreSQL service，远端 CI 必须按本次提交的实际 run 单独验收。历史初始化提交 fb22306 的 Ubuntu CI 37906959016 曾独立回读确认 success，不能代替新提交结果。

## 阻塞与下一顺序

- ⛔ A06 SonnetDB 完整模型：IdentityUserPasskey.Data 的 EF JSON 容器缺少 Provider 存储类型，模型/迁移生成退出 1，尚未执行建表。无需 ABP/EF CLI 的纯 Provider 复现同样失败，同模型 SQLite 退出 0、JSON 类型 TEXT；复现位于 samples/SonnetDB.Abp.JsonColumnRepro/。
- 已查重并创建 SonnetDB issue #266：https://github.com/IoTSharp/SonnetDB/issues/266。独立 GET 回读确认 open，完整独立 csproj/C# 正文与 docs/issues/A06-ef-json-columns.md 一致；接收方无需取得本仓库未提交内容。该通用 Provider 需求由 SonnetDB 研发，适配层不删除 Passkey 或修改 ABP 来绕过。
- SQLite 完整登录写入用户状态后，安全日志 requiresNew 事务遭遇 database is locked；保留 a06-http-test-1 原失败，没有关闭事务、安全日志或授权。完整对照改用 PostgreSQL，SQLite 只计局部验证。
- 上游发布修复的公开包后，先固定版本重跑相同 JSON 复现，再生成 SonnetDB 迁移、实际建库/种子、登录/CRUD/分页和重启验证。SonnetDB 迁移程序集目前仅含标记类型；A06 整体仍为 ⛔。
- 后续按 ROADMAP：S01/A06 → S02–S07/A18–A23 → A02/A04/A03 关系合同/模块/Server → S08/A24 与 A08 文档 → S09–S12/A25–A28 与 A10 事件 → S13/A29 与 A11 后台任务 → A07/A09/A12–A15 剩余模型 → A16 联合验收 → A05 交付。各例子有独立任务和结果；受阻时登记 issue，可继续无依赖的下一例，不把 PostgreSQL 成功推导为 SonnetDB 完整应用成功。

未验：SonnetDB 标准完整模型迁移/种子/Identity、全部 Administration 模块、复杂查询、其他八模型适配、真实 SonnetDB Server、每租户独立数据库、恢复/压力/长稳、真实浏览器点击与正式发布。ABP/EF 集成不声明 Native AOT 支持。许可证和适配包发布策略仍需维护者确定；本轮未发布 NuGet、未提 ABP PR、未新建分支/子智能体/额外聊天/自动化。

## 资源与失败保留

- 提交门禁临时恢复已核验身份的 PostgreSQL 容器，记录本轮 StartedAt、PID、创建时间、命令和任务 label；20 项测试后独立确认 GUID 测试库剩余 0，并恢复 running=false/PID=0，保留 Demo 数据卷。证据 a17-commit-postgres-ownership.json/a17-commit-postgres-cleanup.json；只处理本任务恢复的容器，未删除共享镜像/缓存。
- A17 未启动数据库、Web 或消息服务，未安装工具。新长命令均记录身份；最终核对本轮 15 个已记录进程身份匹配存活数为 0。仅本任务创建的两个临时 ZIP 和微试目录已清理，保留忽略的来源快照、导入脚本和成功/失败记录；证据 a17-process-and-resource-audit.json。不修改上轮保留的 Demo 容器或共享缓存。
- A17 首个微试遇到 ZIP 中 readme.md 的大小写差异；后续逐文件验证发现 Git archive 默认换行转换导致 Dapper 文件与原 Git blob 不一致。按准确大小写与命令范围 core.autocrlf=false/core.eol=lf 修正，已核验完成的文件逐项复核后继续，未删除或覆盖用户文件；最终 1662 个原文件都与原 Git blob 相同。这些是来源导入问题，没有向 SonnetDB 再发需求。
- 本机 PowerShell 7.6.6、.NET SDK 10.0.401；使用已有 EF CLI 10.0.5、Node/npm 和 Docker，未安装或升级工具。长命令通过 eng/Invoke-CheckedCommand.ps1 有界执行，记录 PID/创建时间/命令行/父进程并只清理核验归属资源。
- 已记录 269 个进程身份、253 个不同 PID 的最终审计匹配存活数为 0（a06-process-final-audit.json）；不宣称未观察后代全部已证明不存在。BookStore GUID 测试数据库和专属临时目录已清理，PostgreSQL 独立回读测试库剩余 0。
- 本任务 PostgreSQL 容器 sonnetdb-abp-a06-postgres：ID 9988ec9fdfc0fe3a847aaf0cec0c597951f9612ea4d6ac6673fb58ea5a81c75d；创建 2026-10-09T10:16:35.385690159Z；label sonnetdb.abp.task=A06-20261009；127.0.0.1:54381，512 MiB/2 CPU。核验完整身份后停止，独立回读 running=false、PID=0，数据卷与 BookStore Demo 数据保留；账本为 a06-postgres-resource-ledger.json/a06-postgres-cleanup.json。
- 恢复本机 Demo：docker start sonnetdb-abp-a06-postgres，随后按 samples/BookStore/DEMO.md 运行 Migrator/Web。数据库连接为 Host=127.0.0.1;Port=54381;Database=BookStore;Username=sonnetdb_abp;Password=sonnetdb_abp_demo，仅用于本地演示。
- 日志、TRX、数据库、bin/obj、node_modules、wwwroot/libs 和官方稀疏来源快照位于忽略产物中；保留可复核证据和数据，不删除共享镜像/缓存或用户资源。本轮未修改 SonnetDB 引擎。

来源克隆/包还原直连失败、依赖冲突、空值/Mapperly 映射、Web 入口/静态 manifest、首个 Kestrel 窗口连接拒绝及数据库失败均保留，成功记录不覆盖旧失败。外网失败按任务范围代理重试，凭据不进入日志或仓库。下一位接手先读本文件、ROADMAP、git status 与 origin/main；按实际新提交核对 CI，再推进下一例迁移，不删除上述数据或将入库视为已运行。
