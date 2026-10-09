using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SonnetDB.Abp.EntityFrameworkCore;

/// <summary>
/// 声明 SonnetDB 的 ABP Entity Framework Core 集成依赖。
/// 应用通过 AbpDbContextOptions.UseSonnetDB 显式选择数据库 Provider。
/// </summary>
[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public sealed class SonnetDbAbpEntityFrameworkCoreModule : AbpModule;
