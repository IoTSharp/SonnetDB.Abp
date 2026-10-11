# A06 非 JSON 合同探测

公开固定依赖、独立小模型，检查完整 A06 模型被 JSON #266 阻塞时仍可验证的数据库合同。结果与 issue 对应见 [缺口分析](../../docs/issues/A06-sonnetdb-gap-analysis.md)。不替代标准迁移、完整 ABP 模块、HTTP 或真实 Server 验收；当前多个 SonnetDB 场景故意以失败退出。

```powershell
dotnet restore SonnetDB.Abp.slnx
dotnet build samples/SonnetDB.Abp.A06ContractProbe -c Release --no-restore
./eng/Test-A06Contracts.ps1 -Cases @('crud') -TimeoutSeconds 80
./eng/Test-A06Contracts.ps1 -Cases @('contains-percent','binary-key','named-foreign-key','idempotent-script') -TimeoutSeconds 300
```

只用 PowerShell 7。先运行两个 CRUD 的小型检查再扩展；脚本最多两个 Provider、18 个场景，总墙钟最长 900 秒，每个命令 40 秒、程序 25 秒取消预算。每次使用新 label，原失败回执不覆盖。每个场景创建 GUID 标记目录，DbContext 释放后核验归属并回收；进程身份与清理由 Invoke-CheckedCommand 记录。

直接运行时接受 `Sqlite|SonnetDB` 和一个场景，退出 0 表示该合同通过，1 表示失败，2 表示参数错误。`history` 只测历史表操作；`idempotent-script` 测脚本重放；`requires-new` 测两连接独立事务。SQLite 本身不支持幂等脚本，且独立写事务/跨 offset 查询存在限制，均按原结果报告。

日志、数据库、bin/obj 均为忽略的本地验证产物，不提交。样例不声明 Native AOT 支持。
