# 架构边界

应用 → SonnetDB.Abp.EntityFrameworkCore → SonnetDB.EntityFrameworkCore → SonnetDB ADO.NET / 引擎。

ABP 管理依赖注入、默认仓储、连接串解析、租户上下文、工作单元、审计和过滤；适配层负责选择 Provider 和传递已有连接。既有连接优先，以保留同库多 DbContext 共享事务的标准路径。

适配仓库通过公开 NuGet 包引用数据库 Provider，不存在跨仓库 ProjectReference。通用查询翻译、SQL、迁移或事务错误属于 SonnetDB；ABP 注册、上下文配置与连接复用属于此仓库。禁止将引擎复制到这里维护。

首先固定 .NET 10、ABP 10.7.0、EF Core 10.0.12 和 SonnetDB.EntityFrameworkCore 4.0.0；扩展支持范围前，分别取得对应版本和部署方式的测试证据。

初期样例是单进程控制台，方便复现持久化和事务；不附带庞大的 ABP UI/认证应用。迁移、内置模块、远程与独立租户库保留独立验收。

参考：ABP 官方 EF Core/Other DBMS 文档，以及版本 10.7.0 对应的 Provider 扩展实现。源代码参考用于核对公共入口，不复制 ABP 源码或引入其数据库实现。
