# A06 EF 迁移合同回执

2026-10-11 使用公开 SonnetDB.EntityFrameworkCore 4.0.0、EF Core 10.0.12 验证并发布：

- [SonnetDB #272](https://github.com/IoTSharp/SonnetDB/issues/272)：建表遗漏外键名，按原名 DROP 返回成功但实际约束仍生效。完整独立代码及实测见 [命名外键正文](A06-ef-foreign-key-name.md)。
- [SonnetDB #273](https://github.com/IoTSharp/SonnetDB/issues/273)：接受 Idempotent 后返回无迁移历史保护脚本，首次成功、重放失败。完整独立代码及实测见 [幂等脚本正文](A06-ef-idempotent-script.md)。

两份代码都已独立构建并执行，随后创建 issue，独立 GET 核对正文和 open 状态。它们分别影响后续关系升级和重复脚本部署；初次完整模型仍受 JSON #266 阻塞，不将这两项描述为所有初次建库失败。

完整矩阵、现有 #227 复用范围、待验迁移锁/恢复、原始失败及资源交接见 [A06 缺口分析](A06-sonnetdb-gap-analysis.md)。
