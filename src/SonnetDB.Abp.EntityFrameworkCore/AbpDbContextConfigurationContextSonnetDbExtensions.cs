using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SonnetDB.EntityFrameworkCore.Infrastructure.Internal;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Volo.Abp.EntityFrameworkCore;

/// <summary>提供使用 ABP 连接解析结果和工作单元已有连接的 SonnetDB 配置。</summary>
public static class AbpDbContextConfigurationContextSonnetDbExtensions
{
    /// <summary>使用工作单元已有连接，或使用当前解析出的连接字符串。</summary>
    /// <param name="context">ABP DbContext 配置上下文。</param>
    /// <param name="sonnetDbOptionsAction">可选的 Provider 配置。</param>
    /// <returns>配置后的 EF Core 选项构建器。</returns>
    public static DbContextOptionsBuilder UseSonnetDB(
        this AbpDbContextConfigurationContext context,
        Action<SonnetDbDbContextOptionsBuilder>? sonnetDbOptionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.ExistingConnection is { } connection
            ? context.DbContextOptions.UseSonnetDB(connection, sonnetDbOptionsAction)
            : context.DbContextOptions.UseSonnetDB(context.ConnectionString, sonnetDbOptionsAction);
    }

    /// <summary>使用工作单元已有连接，或使用显式传入的连接。</summary>
    /// <param name="context">ABP DbContext 配置上下文。</param>
    /// <param name="connection">没有工作单元已有连接时使用的连接。</param>
    /// <param name="sonnetDbOptionsAction">可选的 Provider 配置。</param>
    /// <returns>配置后的 EF Core 选项构建器。</returns>
    public static DbContextOptionsBuilder UseSonnetDB(
        this AbpDbContextConfigurationContext context,
        DbConnection connection,
        Action<SonnetDbDbContextOptionsBuilder>? sonnetDbOptionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(connection);
        return context.DbContextOptions.UseSonnetDB(context.ExistingConnection ?? connection, sonnetDbOptionsAction);
    }
}
