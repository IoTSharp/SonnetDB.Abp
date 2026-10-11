# A06 SonnetDB 迁移需求有界查重记录

> 本文保留发布前快照。主任务随后补充四次定向检索，实际探测并新建 #270–#273，独立 GET 核对状态与正文；最终映射见 [A06 缺口分析](A06-sonnetdb-gap-analysis.md)。

查重快照：2026-10-11 09:57:43（Asia/Shanghai）。对象为 `IoTSharp/SonnetDB` 的 open/closed issue，范围是 A06 官方 BookStore 迁移除 JSON 映射 [#266](https://github.com/IoTSharp/SonnetDB/issues/266) 外的数据库与 EF Provider 需求。JSON 复现、已知阻塞和验收合同仍见 [A06-ef-json-columns.md](A06-ef-json-columns.md)。

本次查重支持复用现有 DDL 事务等需求，并区分数据库 SQL 能力与 EF 消费层接线。没有在查重阶段运行模型、迁移、数据库或应用；未找到对应 issue 的子点保持待核验，不能据此宣布新增运行故障。以下状态来自本次 GET 快照，不表示持续监控结果。

读取采用单页 `repos/IoTSharp/SonnetDB/issues?state=all&per_page=100&page=1`，返回 100 项，其中 77 个 issue、23 个 PR；PR 不纳入需求查重。随后仅执行下列三次搜索，每次固定 `per_page=20&page=1`，均返回 `incomplete_results=false`。去重后共 78 个 issue，包含每项 number、title、body、url、state 与创建/更新时间。没有继续分页，因此该结果是有界核对，不能视为全仓库穷尽查重。

| 搜索 query | API total_count | 本次返回项数 |
| --- | ---: | ---: |
| `repo:IoTSharp/SonnetDB is:issue EF` | 3 | 3 |
| `repo:IoTSharp/SonnetDB is:issue 嵌套` | 10 | 10 |
| `repo:IoTSharp/SonnetDB is:issue 二进制` | 1 | 1 |

执行前先以 `gh --version` 微试。四个 API 请求各自直连失败后，只通过命令范围的 `http://127.0.0.1:7890` 代理重试一次；总计 9 个 `gh` 命令，包含微试及失败尝试。所有外部命令通过 `eng/Invoke-CheckedCommand.ps1` 执行，微试预算 10 秒、API 命令预算各 60 秒；任务总预算 10 分钟，最多一个列表页、三次定向搜索，没有无界重试或轮询。执行使用 PowerShell 7.6.6，保留每次独立 label 的失败与成功回执。

| 检查对象 | 快照 issue / 状态 | 重叠关系与 A06 使用边界 |
| --- | --- | --- |
| 关系 DDL 事务 | [#227](https://github.com/IoTSharp/SonnetDB/issues/227) / open | 现有需求已覆盖关系表和索引的 DDL 提交、回滚及多步迁移原子性。相同引擎能力复用该 issue；EF 迁移事务接线仍需独立验证。 |
| 存量主键演进 | [#226](https://github.com/IoTSharp/SonnetDB/issues/226) / open | 已有主键重命名和定义变更需求，可能影响后续结构升级；不能自动当作 A06 首次建库故障。 |
| 建表时命名外键 | [#188](https://github.com/IoTSharp/SonnetDB/issues/188) / closed | 已有对应通用 DDL 需求。A06 应复测固定公开包及实际 EF 生成 SQL，不能仅由关闭状态推导兼容，也不重复申请同一语法能力。 |
| 索引选项与约束目录 | [#250](https://github.com/IoTSharp/SonnetDB/issues/250) / open | 覆盖真实约束目录、索引 ASC/DESC/INCLUDE 与自增起点；正文未明确覆盖 EF filtered unique 索引生成或二进制主键映射。 |
| Decimal 参数精度 | [#235](https://github.com/IoTSharp/SonnetDB/issues/235) / open | 已有参数及数值字面量精度需求。官方 `Book.Price` 为 `float`，须先确认实际 decimal 模型路径，才可列为当前 A06 阻塞。 |
| DECIMAL ALTER 精度 | [#241](https://github.com/IoTSharp/SonnetDB/issues/241) / open | 已覆盖 ADD/ALTER 时精度和小数位丢失，属于有实际字段演进时的条件性重叠。 |
| DECIMAL 精度与小数位执行 | [#264](https://github.com/IoTSharp/SonnetDB/issues/264) / open | 已覆盖写入约束、舍入/拒绝及溢出原子性；不另建同一通用精度需求。 |
| DECIMAL 原生类型 | [#181](https://github.com/IoTSharp/SonnetDB/issues/181) / closed | 基础类型需求已经关闭；它与仍 open 的参数精度、ALTER 保真和约束执行分别验收。 |
| DateTime 参数精度 | [#236](https://github.com/IoTSharp/SonnetDB/issues/236) / open | 已覆盖亚毫秒参数精度。A06 日期/审计路径若复现同一缺口则复用；本次查重未动态验证该路径。 |
| SQL 字符串函数 | [#174](https://github.com/IoTSharp/SonnetDB/issues/174) / closed | 属于通用 SQL 函数需求；关闭状态不验证 EF 的 `ToUpper`、`Contains` 方法翻译、转义和 NULL 语义。 |
| RIGHT/FULL/CROSS JOIN | [#177](https://github.com/IoTSharp/SonnetDB/issues/177) / closed | 属于连接类型扩展，不能直接当作 BookStore 普通 INNER/LEFT JOIN 的故障；EF 投影和分页仍需消费层复测。 |
| Int64 LIMIT/OFFSET | [#256](https://github.com/IoTSharp/SonnetDB/issues/256) / open | 官方作者仓储 `skipCount`、`maxResultCount` 均为 `int`，更大分页范围不是当前基线的自动阻塞项。 |
| Serializable 隔离 | [#225](https://github.com/IoTSharp/SonnetDB/issues/225) / open | 已有隔离级别合同需求；须先确认具体 ABP 路径是否请求 Serializable。 |
| UPDATE JOIN 连续缓冲写 | [#246](https://github.com/IoTSharp/SonnetDB/issues/246) / open | 已有批量联接更新需求；本次没有证据说明普通 BookStore CRUD 使用该入口。 |

本次列表和关键词搜索没有找到明确覆盖下列 EF/ABP 消费层合同的专属 issue。它们是待与源码或最小复现合并的核对点，不是查重阶段已证实的故障：

1. 标准 EF migrations 的 SQL 生成、迁移历史表、迁移锁，以及首次迁移、重复迁移和升级生命周期。涉及引擎关系 DDL 事务的部分关联 #227；EF Provider 自身缺口与该引擎需求分别说明。
2. Passkey 二进制主键的列类型、参数与主外键映射。它与 #266 的 JSON 容器类型映射分别核验，不能把 JSON 模型失败推导为二进制路径已实测失败。
3. 账户规范化查询与作者过滤中的 `ToUpper`/`Contains` 翻译、转义，以及业务 JOIN 投影、稳定排序和 `int` 分页。SQL 内置函数存在不代表对应 LINQ 表达式已被 Provider 翻译。
4. ABP `requiresNew` 使用的独立连接事务、安全日志写入与外层事务完成行为。独立连接上的新事务与同一连接嵌套事务分别核对；保留的 SQLite 写锁失败不能直接归因于 SonnetDB。

后续新 issue 应给出固定公开包、最小模型或 SQL、实际结果、预期合同和复测条件；如果只有源码核对，要明确标注静态证据。已经被现有 issue 覆盖的引擎需求保持同一跟踪入口；只有确认范围不同的 EF Provider 或数据库缺口才独立登记，并交叉关联既有需求。本文件不记录新 issue 的创建或完成，也不把 issue 状态、模型成功、SQL 脚本生成与完整应用验收混称。

本地证据均在忽略的 `artifacts/checks/`，不纳入提交。可复核的归档为：

- `a06-gap-20261011-ci-issue-review.json`：78 个完整 issue、四份原始 JSON 来源、14 行重叠分类及未验证子点。
- `a06-gap-20261011-ci-issues-page1-data.json`：列表原始 JSON；`a06-gap-20261011-ci-search-{ef,nested,binary}-data.json`：三次搜索原始 JSON。
- `a06-gap-20261011-ci-*.json`、对应 `.stdout.log`/`.stderr.log`：独立命令回执及成功/失败日志。
- `a06-gap-20261011-ci-resource-audit.json`：2026-10-11 09:58:11（Asia/Shanghai）资源复核，9 个已完成命令回执、7 个完整进程身份，身份匹配存活数 0；不声明未观察后代已经全部证明不存在。

该只读查重阶段没有改源码、执行 restore/build/test、启动服务、创建临时文件、发布 issue 或评论。原有工作区修改保留；本文件依据上述归档整理，没有新增外网读取。
