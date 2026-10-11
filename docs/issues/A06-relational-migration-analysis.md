# A06 除 JSON 容器外的关系迁移分析草稿

> 本文保留静态分析阶段与探测建议。随后公开包实测已确认 R01/R02，创建 #272/#273；R03/R04 保持待验/复用现有需求。最终矩阵与 issue 回执见 [A06 缺口分析](A06-sonnetdb-gap-analysis.md)。

日期：2026-10-11（Asia/Shanghai）。范围：官方 BookStore 的数据库模型和标准迁移需求，仍属于 A06。本文是只读静态分析与公开包探测建议，不把源码提示写成 `SonnetDB.EntityFrameworkCore 4.0.0` 已复现失败，也不宣布 A06 已解锁。

## 当前证据边界

- #266 已覆盖 ComplexProperty/Owned JSON 容器与实际 Identity Passkey 的完整合同，不重复提交同一 JSON 问题。
- SQLite/PostgreSQL 迁移和模型快照用于确认官方应用需要什么；不能直接把其 Provider 专属类型或 SQL 当作 SonnetDB 的迁移执行输入。
- 本文参考 `D:/source/SonnetDB` HEAD `a1eba7397eb1457d92fd471fac9121eec2db8a85` 的 Provider 与引擎源码，只读，不修改。当前源码可能包含公开 4.0.0 之后的实现，不能证明公开包已具备或缺少相同能力。
- 本子任务没有运行 restore/build/test 或数据库 probe。下列非 JSON 项尚无本轮公开包失败回执；发布新 issue 前应附独立复现、真实结果及查重结论。

## 官方初始迁移需要的合同

`samples/BookStore/src/Acme.BookStore.PostgreSql.Migrations/Migrations/20261009101640_InitialPostgreSql.cs` 与 SQLite 的 `20261009095527_InitialSqlite.cs` 均含 **40 张表、51 个索引、21 个外键、10 个唯一索引**。两份初始迁移都没有 `EnsureSchema`、`InsertData`、过滤索引或独立 `UniqueConstraint` 操作。

| 实际需要 | 官方迁移定位 | SonnetDB 复测重点 |
| --- | --- | --- |
| Guid 主键、可空租户键、字符串 | PostgreSQL 初始迁移第 17–21、198–214 行等 | 从 SonnetDB 模型生成本地方言；Guid 参数、读取与索引值一致 |
| float 价格 | 同文件第 813–838 行，`AppBooks.Price` 为 `float/real` | FLOAT 参数和读取；不能把 decimal 的缺口称为本例价格字段阻塞 |
| DateTimeOffset | 同文件第 431、436–437、754 行 | 时区/精度、参数、读取、排序；DDL 成功不等于值合同通过 |
| 二进制主键 | 同文件第 727–745 行，`AbpUserPasskeys.CredentialId`，最大长度 1024 | 不依赖 JSON，先独立验证 BLOB 主键和二进制参数/相等查询/重复拒绝 |
| 复合主键 | 同文件第 603、649、712、758 行 | 组织/角色关联、租户连接串、用户组织关联、密码历史的完整复合键 |
| 多列唯一索引 | 同文件第 980–990、1007–1034 行 | 3/4/6 列索引；重复非空键拒绝、不同租户隔离、含 NULL 的既有 SQL 语义 |
| 外键和级联删除 | 同文件第 603–615、712–724、738–744、832–838 行 | 孤儿引用拒绝、删除/更新路径、级联与事务回滚、约束名称持久化 |
| 标准迁移历史 | 不在业务初始迁移中，由 EF Provider 提供 | 空库迁移、重复 `MigrateAsync`、重启读取历史、失败不得登记为已应用 |

上述操作是否成功需要公开包实际探测。当前源码已包含复合键、复合索引、BLOB、ON DELETE CASCADE 的语法/实现，不能仅据功能名称提交“完全不支持”的 issue。`TableIndexCodec.cs:190–196` 对含 NULL 的唯一索引跳过编码，需对照同一模型实际语义，也不能直接称为重复约束失效。

## 优先探测：具备具体源码线索的候选

### R01：建表外键的名称与后续删除是否一致

官方初始迁移为每个外键提供名字，例如 `FK_AppBooks_AppAuthors_AuthorId`。当前源码 `SonnetDbMigrationsSqlGenerator.cs:52–55` 在 CREATE TABLE 中调用 `ForeignKeyConstraint`；该方法第 445–451 行只输出 `FOREIGN KEY (...) REFERENCES ...`，没有输出 `operation.Name`。但第 265–268 行的 `DropForeignKeyOperation` 使用原操作名执行 `DROP CONSTRAINT`。解析器 `SqlParser.cs:1952–1994` 支持接收并保留显式约束名，因此这里存在 Provider 名称传递遗漏的静态线索。

公开包最小探测：仅用 EF Core 与 SonnetDB 4.0.0，生成父/子表及名字固定为 `FK_ProbeChild_ProbeParent_ParentId` 的 `CreateTableOperation.ForeignKeys`；保留生成 SQL，执行后回读 schema，再生成并执行同名 `DropForeignKeyOperation`，最后验证是否允许无父行插入。分别记录建表、约束实际名字、删除与实际约束语义，不能只判断 SQL 字符串。

如果公开包实际证明名称丢失且同名删除失败，可提交独立 Provider 迁移 issue。它影响官方模型的后续迁移；是否阻塞初次建表要按实际结果单独写，不扩大为所有外键不可用。

### R02：Idempotent script 与重复 MigrateAsync 的不同合同

当前源码 `SonnetDbHistoryRepository.cs:229–238` 的 `GetBeginIfNotExistsScript`、`GetBeginIfExistsScript` 和 `GetEndIfScript` 返回空字符串；`SonnetDbMigrationsSqlGenerator.cs:30–32` 的普通 CREATE TABLE 也没有 IF NOT EXISTS。这提示 `MigrationsSqlGenerationOptions.Idempotent` 可能生成没有 migration-history 条件保护的脚本。

公开包最小探测：建立一条没有 JSON 的单表 EF migration，调用 `IMigrator.GenerateScript` 并指定 Idempotent；在专属空库完整执行该脚本两次，再重启读取应用历史和数据。另用两个新 DbContext 对同一专属库连续执行 `Database.MigrateAsync` 两次，作为单独的顺序幂等合同。脚本重放失败不等于 `MigrateAsync` 重跑失败，后者会读取历史决定待应用项。

Microsoft Learn《应用迁移》的“幂等 SQL 脚本”说明：脚本检查迁移历史，只应用缺少的迁移；该能力取决于数据库 Provider，SQLite 也没有此能力。因此不能要求所有 Provider 必须实现，而应要求公开包支持时保护重放，未支持时明确拒绝该选项，不能静默返回会重复执行 DDL 的脚本。是否新建 issue 需查重现有迁移脚本支持边界。

### R03：迁移锁与同时启动两个 Migrator

当前源码 `SonnetDbHistoryRepository.cs:218–225` 返回 `NoopMigrationsDatabaseLock`，第 288–309 行没有实际互斥操作。该实现是并发初始化风险线索；不能只从方法名推断公开包竞争已经发生。

Microsoft Learn《应用迁移》的“迁移锁定”说明 EF Core 9 以后，Migrate/MigrateAsync 在应用迁移时使用数据库范围的锁；具体锁定机制因 Provider 而异，手工执行的 SQL 脚本不在该锁范围。公开包 probe 应分别验收 Provider 的运行时迁移锁与脚本部署合同。

公开包最小探测：先验证一条无 JSON migration 能顺序应用，然后以两个独立 DbContext/连接对同一专属库同时执行 MigrateAsync，使用有界 barrier 或拦截器使双方在 history 判断附近相遇；记录双方退出、历史行数、表/索引一致性及重启回读。若首次竞争确证失败，保留原失败后再给出恢复结果，不能只保留重试成功。

该项应与 R02 分开。一次并发 PASS 不能证明互斥合同；源码 Noop 也不能直接当作公开包已发生数据破坏。

### R04：多 DDL 失败后的部分建表与恢复

当前源码 `SonnetDbMigrationsSqlGenerator.cs:477–478` 将 DDL 标为 `suppressTransaction: true`。官方初始迁移有 40 表与 51 索引；若中途失败，必须确认实际留下的表与历史，不能把后来 HTTP 200 或某个表存在视为迁移完成。

公开包最小探测：独立 migration 先建立一张表，再执行确定失败的 SQL；保留数据库和历史的实际快照，重启后检查未被记为已应用，并观察同一迁移重跑是否会因已存在表失败。不要删除用户库或手工修改 history。此项首先是恢复边界验收；非事务 DDL 并不自动构成 bug，issue 应描述已证不一致/不可恢复行为和明确预期。

## 静态关注，但不是本例当前已证阻塞

| 关注点 | 当前源码线索 | 为何不能直接作为 A06 初始迁移失败 issue |
| --- | --- | --- |
| `CreateTableOperation.UniqueConstraints` | 自定义 CreateTable 只遍历 PK/FK/CHECK，未遍历 UniqueConstraints | 官方初始迁移用 10 个唯一索引，没有该操作；可以另作独立生成/重复值探测 |
| 过滤/降序索引 | CreateIndex 第 217–228 行只使用唯一标记和列名 | 本例两份初始迁移没有 Filter/IsDescending，不应伪造本例触发 |
| decimal / DateOnly / TimeOnly 等额外类型 | TypeMappingSource 的 CLR 字典第 39–52 行不含这些类型 | 本例初始迁移实际 CLR 类型没有这些字段；需要业务真实需求或独立通用 issue |
| SQL Server/SQLite/PostgreSQL 专属列类型 | SonnetDB 原生映射为 INT/FLOAT/BOOL/STRING/BLOB/DATETIME | 必须为 SonnetDB 重新生成迁移；直接执行 uuid/bytea/jsonb/nvarchar 失败不是正确迁移的证据 |
| 长度/默认值合同 | TypeMappingSource 使用基础类型；当前源码有 StringLengthConvention | 当前源码新增约束不能证明公开 4.0.0 已覆盖；需模型与真实写入复测，不能只看 DDL 文本 |
| 新增/修改 AUTO_INCREMENT、计算列、改主键等升级操作 | MigrationsSqlGenerator 第 98–130 行及 TableSchema 相应限制 | 本例初始迁移没有这些 ALTER 操作；不要把明确的后续范围混作本次建库阻塞 |
| Schema、序列、迁移 Seed SQL 字面量 | 自定义生成器覆盖并不完整 | 本例没有 EnsureSchema/序列/InsertData；ABP 种子是运行时参数化业务路径，不能套用 InsertData 问题 |

## 发布 issue 的最小证据

每个候选使用独立目录/数据库和公开固定依赖，无 ABP/引擎源码引用；保留操作/模型、生成 SQL、异常全文、退出码、数据库和 history 回读、版本与资源账本。先搜索同义关键字并读取最相近已有 issue；同一根因补充既有 issue，不以不同字段名重复提交。新 issue 明确写“初始建表失败”“后续迁移失败”“并发风险已复现”或“通用能力缺口”，不要把尚未探测的矩阵全部当成 A06 事实。

最终以主任务公开包 probe 与独立 GET 回读的 issue 链接补充本文。未成功验证的实际 SonnetDB 完整 BookStore、Server、恢复、Administration 模块和正式发布仍保持未验状态。
