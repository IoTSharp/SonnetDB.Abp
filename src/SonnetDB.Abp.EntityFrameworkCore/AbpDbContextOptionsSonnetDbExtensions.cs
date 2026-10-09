using SonnetDB.EntityFrameworkCore.Infrastructure.Internal;

namespace Volo.Abp.EntityFrameworkCore;

/// <summary>提供 ABP DbContext 选项的 SonnetDB 配置入口。</summary>
public static class AbpDbContextOptionsSonnetDbExtensions
{
    /// <summary>将默认 DbContext 配置为使用 SonnetDB。</summary>
    /// <param name="options">ABP DbContext 选项。</param>
    /// <param name="sonnetDbOptionsAction">可选的 Provider 配置。</param>
    public static void UseSonnetDB(
        this AbpDbContextOptions options,
        Action<SonnetDbDbContextOptionsBuilder>? sonnetDbOptionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Configure(context => context.UseSonnetDB(sonnetDbOptionsAction));
    }

    /// <summary>将指定 DbContext 配置为使用 SonnetDB。</summary>
    /// <typeparam name="TDbContext">ABP DbContext 类型。</typeparam>
    /// <param name="options">ABP DbContext 选项。</param>
    /// <param name="sonnetDbOptionsAction">可选的 Provider 配置。</param>
    public static void UseSonnetDB<TDbContext>(
        this AbpDbContextOptions options,
        Action<SonnetDbDbContextOptionsBuilder>? sonnetDbOptionsAction = null)
        where TDbContext : AbpDbContext<TDbContext>
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Configure<TDbContext>(context => context.UseSonnetDB(sonnetDbOptionsAction));
    }
}
