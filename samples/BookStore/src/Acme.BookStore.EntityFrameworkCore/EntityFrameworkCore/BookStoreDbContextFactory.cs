using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Acme.BookStore.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class BookStoreDbContextFactory : IDesignTimeDbContextFactory<BookStoreDbContext>
{
    public BookStoreDbContext CreateDbContext(string[] args)
    {
        BookStoreEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration(args);

        var builder = new DbContextOptionsBuilder<BookStoreDbContext>();
        BookStoreDatabaseProvider.Configure(builder, configuration);

        return new BookStoreDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return builder.Build();
    }
}
