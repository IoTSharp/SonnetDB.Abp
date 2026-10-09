using Microsoft.Extensions.DependencyInjection;
using SonnetDB.Abp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace SonnetDB.Abp.Sample;

/// <summary>注册样例 DbContext、默认仓储和 SonnetDB Provider。</summary>
[DependsOn(typeof(SonnetDbAbpEntityFrameworkCoreModule), typeof(AbpAutofacModule))]
public sealed class DemoModule : AbpModule
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<DemoDbContext>(options => options.AddDefaultRepositories());
        context.Services.AddAbpDbContext<DemoReadDbContext>();
        Configure<AbpDbContextOptions>(options => options.UseSonnetDB());
    }
}
