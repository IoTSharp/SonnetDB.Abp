# A06 SonnetDB 迁移缺口与 issue 回执

日期：2026-10-11（Asia/Shanghai）。使用公开 NuGet `SonnetDB.EntityFrameworkCore 4.0.0` 与 EF Core `10.0.12`，独立非 JSON 模型在本地嵌入式数据库实际执行。三个子智能体分别审查模型、应用合同和现有 issue；主智能体集中执行数据库探测、复核独立复现并发布。没有修改引擎、ABP 框架或删除官方模块。

## 需要 SonnetDB 修复的已证缺口

| issue | 已证现象与 A06 影响 | 优先边界 | 完整独立正文 |
| --- | --- | --- | --- |
| [#266](https://github.com/IoTSharp/SonnetDB/issues/266)，已有 | Passkey Complex/Owned JSON 容器缺少存储类型；完整模型与标准迁移生成失败 | 当前首要建模阻塞，仍未生成完整 SonnetDB 迁移 | [JSON 合同](A06-ef-json-columns.md) |
| [#270](https://github.com/IoTSharp/SonnetDB/issues/270)，新建 | 参数化 `Name.Contains(filter)` 的 `%`、`_` 返回全部五行，反斜杠返回零行；预期均为一行 | 官方作者列表和计数错误；JSON 修复后仍不能按业务语义验收 | [筛选复现](A06-ef-string-contains.md) |
| [#271](https://github.com/IoTSharp/SonnetDB/issues/271)，新建 | BLOB 主键可写入，但普通参数相等查询、Passkey `Any/SequenceEqual` 读取均报 `无法比较 Byte[] 与 String` | 非 JSON Passkey 查询独立失败；没有锁定具体参数/执行层根因 | [二进制复现](A06-binary-query.md) |
| [#272](https://github.com/IoTSharp/SonnetDB/issues/272)，新建 | 建表遗漏模型外键名，按原名 DROP 返回成功；之后孤儿写入仍被自动名 `fk_A06Books_1` 拒绝 | 后续关系升级缺口；初次外键建表与基本约束可用 | [命名外键复现](A06-ef-foreign-key-name.md) |
| [#273](https://github.com/IoTSharp/SonnetDB/issues/273)，新建 | 接受 `Idempotent\|NoTransactions`，首次执行成功，第二次报表已存在；脚本未按 migration history 保护操作 | 重复脚本部署缺口；不等于连续两次 `MigrateAsync` 失败，也不要求必须实现幂等脚本，可明确拒绝不支持选项 | [幂等脚本复现](A06-ef-idempotent-script.md) |

四个新 issue 均只创建一次，随后独立 GET 确认编号、URL、`open` 状态与完整正文一致。回执为 `artifacts/checks/a06-gap-20261011-{create,readback}-{contains,binary,foreign-key,idempotent}.*`；完整代码直接包含在 issue 中，接收方不需要获取本仓库未提交文件。

每份独立代码均另行提取到专属目录，使用显式公开版本构建，0 警告/0 错误；14 次独立运行重现上述结果。SQLite 对照：作者三项、二进制两项、外键删除一项通过；幂等脚本明确抛 NotSupportedException。SonnetDB 七项均按缺陷退出 1。回执为 `a06-gap-20261011-standalone-summary.json`；错误退出保留，不并入常规通过测试。首次二进制正文提取误选了三行说明片段而编译失败，原回执保留；提取完整 Program.cs 后编译与执行结果如上，属于本地证据整理问题。

## 已执行的非 JSON 合同矩阵

入口：[Program.cs](../../samples/SonnetDB.Abp.A06ContractProbe/Program.cs)、[有界执行脚本](../../eng/Test-A06Contracts.ps1)。每个 Provider/场景独立进程、GUID 数据库目录、25 秒程序取消预算、40 秒命令上限；模型、SQL、执行、断言和清理分别记录。以下是小模型合同探测，不是整个 ABP 模块或完整 HTTP 应用验收。

| 场景 | SQLite | SonnetDB 4.0.0 | 验证范围 |
| --- | --- | --- | --- |
| `crud` | PASS | PASS | Guid、float、DateTime 插入/读取/更新 |
| `join-paging` | PASS | PASS | Book–Author JOIN、投影、排序、非零 Skip/Take |
| `contains-percent` | PASS，1 | FAIL，5 | 参数字面 `%`，列表及 Count |
| `contains-underscore` | PASS，1 | FAIL，5 | 参数字面 `_`，列表及 Count |
| `contains-backslash` | PASS，1 | FAIL，0 | 参数字面反斜杠，列表及 Count |
| `identity-includes` | PASS | PASS | 归一化列查找、三个集合 Include、FirstOrDefault；实际七集合仍待完整模型复测 |
| `binary-key` | PASS | FAIL | BLOB 参数相等读取 |
| `passkey-any` | PASS | FAIL | 多用户正例/不存在凭据负例的相关 Any + SequenceEqual；SonnetDB 在正例读取即失败 |
| `composite-key` | PASS | PASS | 三列键正确定位不同 LoginProvider 的 token |
| `unique-null` | PASS | PASS | 四列唯一索引，NULL 对照语义及非空重复拒绝 |
| `foreign-key` | PASS | PASS | 孤儿写入确实因外键冲突被拒绝 |
| `named-foreign-key` | PASS | FAIL | 无外键目标模型；删除后通过真实孤儿插入判断约束效果 |
| `history` | PASS | PASS | 历史表重复创建、插入、回读；未执行标准 MigrateAsync |
| `requires-new` | FAIL，锁冲突 | PASS | 两上下文/独立连接、不同表内层提交、外层回滚；内层日志保留且原作者恢复 |
| `collection-filter` | PASS | PASS | 空/单项/多项集合参数 Contains |
| `role-union` | PASS | PASS | 模拟直授/组织授予的 JOIN + UNION 去重形状，不代表实际角色仓储全覆盖 |
| `idempotent-script` | 明确拒绝 | FAIL，重放 | 无事务脚本首次/重复执行，与迁移历史行核对 |
| `datetimeoffset` | 跨 offset 相等 FAIL | PASS | 非空/可空 Identity 时间字段保存、按 UTC 等价瞬时查询；SQLite 本身限制不报为 SonnetDB 问题 |

SonnetDB 18 个场景中 11 PASS / 7 FAIL，七个失败归并为四个新 issue。SQLite 的锁、幂等脚本能力与跨 offset 相等查询保留独立失败/限制，不当作 SonnetDB 缺口。早期只检查 DROP 命令返回值的弱断言曾记录 PASS；加强为真实写入验收后发现 #272。早期 history probe 混入 SQLite 不支持的幂等 guard 调用也保留原失败；分离合同后，两边 history 均 PASS。

矩阵回执分别为 `a06-gap-20261011-query-summary.json`、`schema-summary.json`、`runtime-summary.json`、`refined-summary.json`、`offset-summary.json` 及两份 CRUD 回执；以 refined 的命名外键/history/Passkey 结果覆盖早期弱合同解释，不覆盖原日志。汇总为 `a06-gap-20261011-contracts-final.json`。

## 复用现有需求与尚未证明的风险

- DDL 事务/多步迁移失败回滚已有 [#227](https://github.com/IoTSharp/SonnetDB/issues/227)，不新建重复需求。本轮没有实际执行完整 40 表迁移或中途失败恢复；它是迁移恢复门禁，不把无事务 DDL 自动表述成所有初次建库失败。
- [#188](https://github.com/IoTSharp/SonnetDB/issues/188) 是已关闭的引擎命名外键语法，[#250](https://github.com/IoTSharp/SonnetDB/issues/250) 是维护目录/索引选项。新 #272 描述 EF 生成时丢失名称及 DROP 的实际无效，范围不同。
- 迁移锁的 Noop 源码线索、并发 Migrator、连续两次 MigrateAsync、完整模型失败恢复尚未以本轮公开包证明，不建立“已复现”issue。JSON 解锁后补标准迁移合同与有界并发测试。
- Book.Price 实际为 float，分页参数为 int；现有 decimal/Int64 能力 issue 不自动列为本例失败。当前官方初始迁移没有 schema、序列、过滤索引、InsertData 或独立 UniqueConstraint 操作。
- 现有八项 SonnetDB 基础集成测试通过；普通 Guid、软删除、审计、并发戳、过滤和共同事务回滚不重新报为缺失。

查重边界：一页 100 条，去除 PR 后 77 个 issue，加三次关键词检索合并为 78 项快照；发布前另检索 Contains、Byte[]、idempotent、外键，各最多 20 项，`incomplete_results=false`。没有找到上述四项相同根因的公开 issue；不是穷尽全库。详见 [查重记录](A06-sonnetdb-issue-dedup.md)、[模型静态分析](A06-relational-migration-analysis.md)、[应用静态分析](A06-application-query-analysis.md)。

## 最终检查与下一步

新增诊断项目已纳入根解决方案。最终源码树完成 restore、format verify（severity warn）、Release build/test，0 警告/0 错误、20/20、无失败/跳过；证据 `a06-gap-20261011-final-{restore,format,build,test}.json` 和 `final-test-totals.json`。这些通过测试与故意失败的合同探测分别记录，A06 仍为受阻、完整迁移仍为 0/13。

等待公开修复包：先固定版本验证 #266 模型/SQL生成，再生成完整 SonnetDB 迁移、实际建库/重复种子、实际账户/Passkey、作者字面筛选、CRUD/分页和宿主重启；升级时验收 #272，选择脚本部署时验收 #273。真实 Server、全部 Administration、并发迁移/失败恢复和正式发布继续独立验收。

保留原始成功/失败日志、TRX 与包还原/构建产物；四个正文提取目录已回收，数据库探测目录剩余 0，PostgreSQL Demo 容器恢复停止并保留数据。最终审计 105 个命令回执、227 个进程身份/216 个 PID，身份匹配存活 0；不推导未观察后代全部不存在。分析完成时没有暂存、提交或推送，用户原有十个 A06 dirty 路径保留；用户随后授权全部提交和推送，交付状态见 HANDOFF 与本地 a06-publish-20261011-state.json 实际回读。
