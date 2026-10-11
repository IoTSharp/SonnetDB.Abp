# A06 JSON 容器模型复现

本程序只构造 EF Core 设计时关系模型并输出 JSON 列的存储类型，不打开连接、不执行迁移或 SQL、不创建数据库。依赖由根 `Directory.Packages.props` 固定，无 ABP 包和 SonnetDB 引擎源码引用。

参数为 `Sqlite|SonnetDB [Complex|Owned]`，区分大小写。省略第二个参数时保留原 `ComplexProperty(...ToJson)` 最小复现及原输出格式；`Owned` 增加官方 BookStore Identity Passkey 的 `OwnsOne(...).ToJson("Data")` 形状。

```text
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- Sqlite
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- SonnetDB
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- Sqlite Owned
dotnet run --project samples/SonnetDB.Abp.JsonColumnRepro -c Release --no-build --no-restore -- SonnetDB Owned
```

Owned 模型按 `samples/BookStore/src/Acme.BookStore.PostgreSql.Migrations/Migrations/BookStoreDbContextModelSnapshot.cs` 中的官方 Passkey 映射构造：`CredentialId` 二进制主键、`TenantId`、用户外键和级联关系，以及 `Data` 中的二进制字段、`DateTimeOffset`、三个布尔值、名称、计数和 `PrimitiveCollection<string>` 传输方式。用户实体仅保留建立外键必需的主键与集合；不复制 ABP 框架实现。列类型由被选 Provider 决定，不手工添加 `HasColumnType` 绕过类型映射。

成功构造模型退出 `0`，Provider 抛出 `InvalidOperationException` 时输出原异常并退出 `1`，无效参数输出 usage 并退出 `2`。SonnetDB 的原失败保持失败，不转换成 expected-failure 成功。

即使两个模型都成功，这也只证明设计时容器映射；不证明 JSON CRUD、参数、查询、事务、实际 Passkey 注册或完整 BookStore 迁移运行。A06 的后续验收合同见 [JSON 能力需求](../../docs/issues/A06-ef-json-columns.md)。
