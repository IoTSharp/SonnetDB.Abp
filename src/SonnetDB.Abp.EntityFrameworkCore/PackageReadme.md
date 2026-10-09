# SonnetDB.Abp.EntityFrameworkCore

SonnetDB 的 ABP EF Core 适配层，依赖 SonnetDB.EntityFrameworkCore 4.0.0、ABP 10.7.0 和 EF Core 10.0.12，目标框架 net10.0。

在应用的 EntityFrameworkCore 模块声明对 SonnetDbAbpEntityFrameworkCoreModule 的依赖，通过 Configure<AbpDbContextOptions>(options => options.UseSonnetDB()) 选择 Provider。连接字符串仍由 ABP 的连接解析器负责；工作单元已有连接优先复用。

本包处于开发阶段，尚未发布。完整兼容性和未验证边界见仓库 README，不声明 Native AOT 或所有 ABP 内置模块兼容。
