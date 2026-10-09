using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.BookStore.EntityFrameworkCore;

/// <summary>集中选择 PostgreSQL 完整基线、SQLite 对照或本仓库 SonnetDB 适配。</summary>
public static class BookStoreDatabaseProvider
{
    /// <summary>按配置注册 ABP Provider，保留 ABP 的连接解析和复用。</summary>
    public static void Configure(AbpDbContextOptions options, IConfiguration configuration)
    {
        if (GetName(configuration) == "SonnetDB")
        {
            options.UseSonnetDB(builder => builder.MigrationsAssembly("Acme.BookStore.SonnetDB.Migrations"));
        }
        else if (GetName(configuration) == "PostgreSql")
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            options.UseNpgsql(builder => builder.MigrationsAssembly("Acme.BookStore.PostgreSql.Migrations"));
        }
        else
        {
            options.UseSqlite(builder => builder.MigrationsAssembly("Acme.BookStore.Sqlite.Migrations"));
        }
    }

    /// <summary>为标准 EF 设计时工具选择与运行时相同的 Provider。</summary>
    public static void Configure(DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");
        if (GetName(configuration) == "SonnetDB")
        {
            options.UseSonnetDB(connectionString, builder => builder.MigrationsAssembly("Acme.BookStore.SonnetDB.Migrations"));
        }
        else if (GetName(configuration) == "PostgreSql")
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            options.UseNpgsql(connectionString, builder => builder.MigrationsAssembly("Acme.BookStore.PostgreSql.Migrations"));
        }
        else
        {
            options.UseSqlite(connectionString, builder => builder.MigrationsAssembly("Acme.BookStore.Sqlite.Migrations"));
        }
    }

    /// <summary>读取并严格校验 Provider 名称，防止拼写错误退回基线。</summary>
    public static string GetName(IConfiguration configuration) => configuration["Database:Provider"] switch
    {
        null or "Sqlite" => "Sqlite",
        "SonnetDB" => "SonnetDB",
        "PostgreSql" => "PostgreSql",
        var value => throw new InvalidOperationException($"Unsupported Database:Provider '{value}'.")
    };
}
