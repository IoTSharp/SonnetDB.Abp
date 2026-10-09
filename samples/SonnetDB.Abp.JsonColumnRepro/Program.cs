using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

if (args.Length != 1 || args[0] is not ("Sqlite" or "SonnetDB"))
{
    Console.Error.WriteLine("Usage: dotnet run -- Sqlite|SonnetDB");
    return 2;
}

try
{
    using var context = new JsonColumnContext(args[0]);
    // 只构建标准迁移所需模型，无数据库文件、ABP 改动或引擎源码依赖。
    var model = context.GetService<IDesignTimeModel>().Model.GetRelationalModel();
    var table = model.Tables.Single();
    Console.WriteLine($"{args[0]}: {table.Name}.Data store type = {table.Columns.Single(column => column.Name == "Data").StoreType}");
    return 0;
}
catch (InvalidOperationException exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}

internal sealed class JsonColumnContext(string provider) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (provider == "Sqlite")
        {
            options.UseSqlite("Data Source=:memory:");
        }
        else
        {
            options.UseSonnetDB("Data Source=json-column-repro");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Passkey>().ComplexProperty(passkey => passkey.Data, data => data.ToJson());
    }
}

internal sealed class Passkey
{
    public Guid Id { get; set; }
    public PasskeyData Data { get; set; } = new();
}

internal sealed class PasskeyData
{
    public string CredentialId { get; set; } = string.Empty;
}
