using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

if (args.Length is < 1 or > 2 || args[0] is not ("Sqlite" or "SonnetDB") ||
    (args.Length == 2 && args[1] is not ("Complex" or "Owned")))
{
    Console.Error.WriteLine("Usage: dotnet run -- Sqlite|SonnetDB [Complex|Owned]");
    return 2;
}

var modelName = args.Length == 2 ? args[1] : "Complex";

try
{
    using var context = modelName == "Owned"
        ? (DbContext)new OwnedJsonColumnContext(args[0])
        : new JsonColumnContext(args[0]);
    // 只构建标准迁移所需模型，无数据库文件、ABP 改动或引擎源码依赖。
    var model = context.GetService<IDesignTimeModel>().Model.GetRelationalModel();
    var tableName = modelName == "Owned" ? "AbpUserPasskeys" : "Passkey";
    var table = model.Tables.Single(table => table.Name == tableName);
    var label = modelName == "Owned" ? $"{args[0]} Owned" : args[0];
    Console.WriteLine($"{label}: {table.Name}.Data store type = {table.Columns.Single(column => column.Name == "Data").StoreType}");
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

// 官方 Identity Passkey 使用 OwnsOne JSON；保留其字段类型、数组与用户外键形状。
internal sealed class OwnedJsonColumnContext(string provider) : DbContext
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
        builder.Entity<IdentityUser>().ToTable("AbpUsers");
        builder.Entity<IdentityUserPasskey>(passkey =>
        {
            passkey.ToTable("AbpUserPasskeys");
            passkey.HasKey(value => value.CredentialId);
            passkey.Property(value => value.CredentialId).HasMaxLength(1024);
            passkey.HasOne<IdentityUser>()
                .WithMany(user => user.Passkeys)
                .HasForeignKey(value => value.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            passkey.OwnsOne(value => value.Data, data =>
            {
                data.ToJson("Data");
                data.PrimitiveCollection(value => value.Transports);
            });
        });
    }
}

internal sealed class IdentityUser
{
    public Guid Id { get; set; }
    public ICollection<IdentityUserPasskey> Passkeys { get; set; } = [];
}

internal sealed class IdentityUserPasskey
{
    public byte[] CredentialId { get; set; } = [];
    public Guid? TenantId { get; set; }
    public Guid UserId { get; set; }
    public IdentityPasskeyData Data { get; set; } = new();
}

internal sealed class IdentityPasskeyData
{
    public byte[] AttestationObject { get; set; } = [];
    public byte[] ClientDataJson { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsBackedUp { get; set; }
    public bool IsBackupEligible { get; set; }
    public bool IsUserVerified { get; set; }
    public string? Name { get; set; }
    public byte[] PublicKey { get; set; } = [];
    public long SignCount { get; set; }
    public string[] Transports { get; set; } = [];
}
