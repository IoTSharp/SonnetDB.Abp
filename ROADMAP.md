# ROADMAP

每项为本项目交付编号，不代表已有 GitHub issue 或 PR。

| 编号 | 目标 | 状态 | 验收 |
| --- | --- | --- | --- |
| A01 | 独立仓库与 EF Core 配置桥接 | 本地实现与最终门禁通过 | 三项目 Release 零警告/错误；完整 restore/Format Check、八项真实本地 ABP 仓储/UOW/过滤测试及样例通过，初始化内容随本次提交保存；远端 CI 单独记录 |
| A02 | 迁移与关系模型边界 | 计划 | Migrate、重复迁移、索引/约束、复杂查询与异常传播 |
| A03 | 远程 Server 与租户数据库 | 计划 | 真实 Server 仓储/UOW、连接解析、多 DbContext、每租户独立数据库隔离 |
| A04 | ABP 内置模块 | 计划 | 逐模块验证 Identity、Permission/Setting Management 与 OpenIddict |
| A05 | 预览包与上游协作 | 计划 | 确定许可证、包发布、版本矩阵及维护者沟通；有通用缺口才提交上游修复 |

优先闭合 A01 的真实结果，再开始 A02；测试失败按所属边界记录，不通过禁用 ABP 过滤或替换标准仓储来制造通过。
