# SonnetDB.Abp 交接记录

## 2026-10-14 A06 / #266 公开修复包消费

本轮从干净main 8006cf2d527a5a46e06b205ddd03c69fe52e5674建立codex/a06-ef266-public-packages，沿用用户对#266代码交付、公开包批准后的消费者固定/复验与满足验收后关闭的授权。根依赖已固定NuGet.org的SonnetDB.EntityFrameworkCore 4.5.0-preview.ef266.5，来源为SonnetDB 1af23d8f299febfdf66b040ab3419980396acf46（#306合入2b079317）。使用仅NuGet.org的配置、全新D:\t\ef266pub5\nuget缓存和空fallback还原，未跨仓库ProjectReference、复制引擎源码或改共享包缓存。

引擎侧发布run38421367503实际六包已接受并公开；公共有效载荷逐项等于获批CI原包，NuGet仓库签名验证全部通过。第七包Testcontainers.SonnetDB因保留命名空间HTTP409被拒，没有重投或更名；这是独立发布阻塞，不把整批记为成功。本仓库依赖的EF Provider/ADO.NET/Core均已公开，不依赖该Testcontainers包。

原统一模型入口5/5通过（SQLite/ SonnetDB ×Complex/Owned，完整BookStore建表脚本），原EF CLI10.0.5保留低于运行时10.0.12的提示。标准InitialSonnetDB迁移、Designer和ModelSnapshot直接生成到本仓库迁移项目；保留原其它Provider迁移。新增公开包账户HTTP与真实IdentityUserStore Passkey存储回归，各自本地1/1、零跳过。HTTP覆盖标准初始/重复迁移、登录、页面/CRUD/分页、应用重建读取与删除；Passkey覆盖二进制/数组/Unicode/时间/uint.MaxValue及多次应用重建。官方业务、UI、模块、授权、事务均未修改，测试不代表浏览器WebAuthn或Server进程重启。

引擎侧另用同源码同版本Server复验真实REST/h2c数据连接，公共消费者与服务器/协议/包字节绑定独立保存在D:\source\SonnetDB\artifacts\online-closure-20261012的ef266pub5-*。提交前最终restore、原Format、Release build/test及远端CI按各自真实收据核验，不用旧20项或定向2项替代完整门禁。测试所需既有PostgreSQL容器必须核完整ID/创建时间/label/image，再临时启动并恢复stopped；保留Demo数据，GUID测试库应清零。

README、ROADMAP、示例索引、DEMO和#266合同已按新范围更新。A06仍进行中、完整示例0/13；#270–#273独立合同、全部Administration、升级/脚本、故障恢复及其它12示例未被自动关闭。旧4.0.0模型失败、SQLite写锁与所有历史FAIL/UNKNOWN保持。#266须在本次公共消费者提交与CI验收完成后由引擎侧关闭；本仓库NuGet适配包未发布，无其它聊天/自动化/子智能体。

当前可提交范围仅公共依赖固定、三个标准迁移文件、两项回归及对应A06文档；日志/TRX、数据库、包、bin/obj与外部探针不提交。最终提交/远端SHA和清理结果以本次ef266pub5回执及Git实际状态为准。本段以下均为前序历史，不能将旧受阻状态作为新固定版本的当前结论。

日期：2026-10-11（Asia/Shanghai）。工作区：D:/source/SonnetDB.Abp。本轮只推进 A06；用户明确授权在不冲突的情况下使用多个智能体。

## 本次 A06 提交与推送交接

- 用户已明确授权提交所有文件并推送。将本轮全部 24 个可提交文件一起保存，包括前序 Owned JSON 复现、SQLite 测试连接修复、两个有界入口、非 JSON 合同项目、四项 issue 正文及路线图/交接记录。日志、数据库、bin/obj 等仍按仓库规则保留为忽略的本地证据，不强制收录。
- 分支 main，提交前 HEAD=e1d363d376726693770d92645e755f45f970e955，origin=IoTSharp/SonnetDB.Abp；提交前 fetch 已成功。本节随 A06 提交保存，最终提交号、远端 SHA、工作区和本次门禁结果以 artifacts/checks/a06-publish-20261011-state.json 与 a06-publish-20261011-gates-summary.json 的实际回读为准。
- 提交前在冻结的源码树执行根解决方案 restore、format verify（severity warn）、Release build/test；标准门禁与已证失败的数据库 probe 分开。保留四项公开 issue #270–#273、JSON #266 和 A06 受阻状态，不以提交或 CI 启动作为完整迁移通过。
- 门禁所需 PostgreSQL Demo 容器按既有账本核对身份，记录本次启动并恢复原停止状态，保留数据；命令进程回执、测试库和清理记录使用新的 a06-publish-20261011-* 标签，原成功/失败证据不覆盖。新提交远端 CI 需要按实际 SHA 和 run 独立验收。

## 本次 A06 非 JSON 缺口分析与 issue 交付

- 用户要求分析还有哪些 SonnetDB 缺口并创建 issue；沿用已明确授权的多个子智能体，各自只写模型/应用/查重与正文文档，主智能体单进程顺序跑数据库探测、核对独立代码并发布。只处理 A06，没有修改引擎、ABP 框架或删除官方模块。
- 新建并独立 GET 回读 SonnetDB #270（参数化 Contains 的 %/_/反斜杠字面语义）、#271（BLOB 普通/Passkey Any 二进制参数读取）、#272（EF 建表丢失外键名，DROP 返回成功但约束仍生效）、#273（接受 Idempotent 的无历史保护脚本，首轮成功重放失败）。编号、open 状态和完整正文分别核对；每项只创建一次。已有 JSON #266 和 DDL 事务 #227 复用，迁移锁/完整失败恢复未证，不重复提报。链接和最小独立正文见 docs/issues/A06-sonnetdb-gap-analysis.md。
- 公开 SonnetDB.EntityFrameworkCore 4.0.0 / EF Core 10.0.12 实际运行 18 个非 JSON 合同，11 PASS/7 FAIL，失败归并为四项新 issue。CRUD/float/Guid/DateTime、JOIN分页、三个集合 Include、复合键、NULL唯一索引、基本外键、history 表、独立日志事务、集合过滤、UNION及DateTimeOffset通过；小模型通过不代表整个 ABP 模块。SQLite 独立写锁、幂等脚本明确拒绝及跨 offset 相等限制保留，不当作 SonnetDB 问题。
- 四份 issue 的完整独立代码另行提取构建，0 警告/0 错误，14 次运行复现问题与对照；第一次二进制正文提取误选说明片段的编译失败保留，修正为完整代码后成功。早期只验 DROP 返回值的弱合同 PASS、history 混入不支持 guard 的失败也保留；加强合同后以 refined/standalone 的真实执行解释为准。原成功/失败回执均未覆盖。
- 新 samples/SonnetDB.Abp.A06ContractProbe 纳入根解决方案，eng/Test-A06Contracts.ps1 先以 CRUD 微试，数量/总时间/单命令/程序取消均有界。最终源码树 restore、format verify、Release build/test 全部通过，0 警告/0 错误、20/20、无跳过。证据 a06-gap-20261011-final-{restore,format,build,test}.json、final-test-totals.json；诊断 FAIL 未伪装为通过测试。A06 仍 ⛔，完整迁移仍 0/13。
- 恢复迁移：先获取并固定公开修复包，验证 #266 的两个 JSON 模型和完整模型/迁移；真实建库与种子后验收 #270 作者过滤、#271 Passkey、账户/CRUD/分页/重启；关系升级和脚本部署分别验收 #272/#273。Server、全部 Administration、并发迁移/恢复和发布仍独立待验。
- 资源：最终门禁核对已有 PostgreSQL Demo 容器身份后启动，记录本次 StartedAt/PID，再恢复 stopped/running=false/PID=0，保留数据，GUID 测试库剩余 0。独立合同目录剩余 0，四个临时正文提取目录按任务归属回收，日志/TRX、bin/obj 作为忽略证据保留。最终审计 105 个命令回执、227 个进程身份/216 个 PID，匹配存活 0；审计自身 checked 回执也清理完成。证据 a06-gap-20261011-resource-audit.json；不宣称未观察后代全部已证明不存在。首次资源审计把 JSON 数组汇总误识别成命令回执而拒绝，失败保留；限定标量回执后，混合输入微试和完整审计通过。
- Git（分析结束时）：仍在 main，HEAD=e1d363d376726693770d92645e755f45f970e955，当时无暂存、无新提交/推送。本次开始时上轮十个 A06 dirty 路径已存在并保留；本次另修改 SonnetDB.Abp.slnx 和三个根文档，新增合同项目、脚本与分析/issue 文档，最终 owned-path 清单以 git status 为准。没有安装工具、修改共享缓存或新建聊天/自动化。用户后续授权提交与推送，见本文件顶部本次交接。

## 前序 A06 结果

- 主智能体集中执行验证，三个子智能体分别负责模型复现、HTTP/SQLite 测试审查与夹具修复、CI 回读与复测脚本；写入目录互不重叠，合并后交叉审查。没有开展 A18 等其他迁移任务。
- 现场回读 NuGet 公开版本，SonnetDB.EntityFrameworkCore 最新仍为 4.0.0；#266 仍 open，updated_at=2026-10-09T10:26:16Z。原 main=e1d363d376726693770d92645e755f45f970e955 的 GitHub CI 37922480045 已回读 completed/success、20/20，仅是既有基线的 CI。
- 原 Complex 最小复现与完整官方 `migrations add InitialSonnetDB` 再次退出 1，仍报 Passkey `Data` JSON 容器存储类型为 null。未生成 SonnetDB 标准迁移或创建数据库；现有 SonnetDB 迁移程序集仍只有标记类型。A06 整体保持 ⛔，完整迁移仍为 0/13。
- 独立 JsonColumnRepro 新增可选 Owned 模式，覆盖实际 Passkey 的二进制主键、用户外键、JSON 字段与字符串数组，保留原 Complex 命令和 0/1/2 退出码。新 eng/Test-A06ModelReadiness.ps1 有数量/时间边界、微试、独立 label 与失败汇总，只检查设计时模型和官方建表 SQL 生成。实际微试成功；完整检查 SQLite Complex/Owned 均成功，SonnetDB Complex/Owned 与完整官方模型均失败，成功 2/失败 3/未执行 0，入口退出 1、allModelsReady=false。汇总为 artifacts/checks/a06-model-readiness-b5cbbadf0a7d456aac31d5504c3bcc08.json；原失败记录保留。
- 首次回归 19/20：官方 SQLite 作者测试的种子初始化在函数注册处报 database is locked；PostgreSQL 完整 HTTP 旅程与 SonnetDB 基础测试成功。夹具改为 GUID 命名共享内存库，keeper 维持库生命周期，各 DbContext 使用独立连接，打开/建表失败释放 keeper。具体活动语句及并发来源未捕获；不与历史 SQLite 完整登录的安全日志写锁混称。业务、ABP 模块、授权、事务和安全日志未变，首个失败与 TRX 保留。
- 最终源码树完成 CI 同级 restore、format verify（severity warn）、Release build/test，全部退出 0，0 警告/0 错误、20/20、无失败/跳过。三支持/基类程序集没有独立测试，不额外计数。证据为 artifacts/checks/a06-resume-20261011-final-{restore,format,build,test}.json、a06-resume-20261011-final-test-totals.json 与 artifacts/test-results/a06-resume-20261011-final/。独立 JSON 模型检查的失败没有计入通过，也未添加跳过的 SonnetDB 应用测试。

## 恢复迁移与资源交接

- 等待 #266 的公开 NuGet 修复版本，先在 Directory.Packages.props 固定新版本，再 restore/Release build 和运行新复测入口。两个 JSON 模型与完整模型成功后，才生成独立 SonnetDB 标准迁移、实际建库/重复种子、真实 Passkey 读写、实际账户 HTTP 登录/CRUD/分页和宿主重启读取；模型、SQL 生成、数据库执行与应用验收分别记录。
- 仍未验收：SonnetDB 完整应用与标准迁移、全部 Administration 模块、真实 Server、恢复、浏览器点击与正式发布。没有修改引擎、跨仓库引用、缓存或 ABP 框架；没有安装/升级工具、新建聊天/自动化或发布包。
- Git：起点 main 工作区干净、HEAD=e1d363d376726693770d92645e755f45f970e955。当前修改仅本轮 A06 的文档、JSON 复现、SQLite 测试夹具与复测脚本；无暂存、无新提交/推送，最终 owned-path 清单以 git status 为准。ROADMAP、CHANGELOG、DEMO、UPSTREAM 和 issue 合同已同步。
- 两次回归均先核对保留的 PostgreSQL 容器完整 ID/创建时间/task label，记录本次 StartedAt/PID 后恢复，finally 核对身份并停止。最终 GUID 测试数据库剩余 0、running=false/PID=0；保留 Demo 数据。证据为 a06-resume-20261011-final-postgres-{ownership,cleanup}.json；容器 ID 与历史账本相同。
- 最终资源审计覆盖 57 个已完成命令回执、194 个不同进程身份/185 个 PID，身份匹配存活数 0；审计命令自己的 checked 回执也确认清理。证据为 artifacts/checks/a06-resume-20261011-resource-audit.json；不宣称未观察后代已全部证明不存在。无待清理专属服务或临时下载；本轮本地验证脚本、日志/TRX、bin/obj 和既有 Demo 数据作为忽略证据保留，不提交、不清理共享缓存。

## 2026-10-09 历史交接

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
