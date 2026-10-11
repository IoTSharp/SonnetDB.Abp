using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

string[] cases = ["crud", "join-paging", "contains-percent", "contains-underscore", "contains-backslash",
    "identity-includes", "binary-key", "composite-key", "unique-null", "foreign-key", "named-foreign-key",
    "history", "requires-new", "passkey-any", "collection-filter", "role-union", "idempotent-script", "datetimeoffset"];
if (args.Length != 2 || args[0] is not ("Sqlite" or "SonnetDB") || !cases.Contains(args[1], StringComparer.Ordinal))
{
    Console.Error.WriteLine("Usage: dotnet run -- Sqlite|SonnetDB " + string.Join('|', cases));
    return 2;
}

var owner = Guid.NewGuid().ToString("N");
var root = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "artifacts", "a06-gap-probe"));
var directory = Path.Combine(root, owner);
Directory.CreateDirectory(directory);
await File.WriteAllTextAsync(Path.Combine(directory, ".owner"), owner);
Console.WriteLine($"PROBE {args[0]} {args[1]} OWNER {owner} DIRECTORY {directory}");
var clock = Stopwatch.StartNew();
using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(25));
var result = 1;
try
{
    await using var context = new ContractContext(args[0], args[1], directory);
    Console.WriteLine("DDL " + context.Database.GenerateCreateScript());
    await context.Database.EnsureCreatedAsync(deadline.Token);
    await RunAsync(context, args[1], deadline.Token);
    Console.WriteLine($"PASS {args[0]} {args[1]} {clock.Elapsed.TotalSeconds:F2}s");
    result = 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
}
finally
{
    try
    {
        var expected = Path.GetFullPath(Path.Combine(root, owner));
        if (!string.Equals(expected, Path.GetFullPath(directory), StringComparison.OrdinalIgnoreCase) ||
            File.GetAttributes(directory).HasFlag(FileAttributes.ReparsePoint) ||
            await File.ReadAllTextAsync(Path.Combine(directory, ".owner")) != owner)
        {
            throw new InvalidOperationException("Refusing cleanup of unverified probe resources.");
        }
        Directory.Delete(directory, recursive: true);
        Console.WriteLine("CLEANED " + directory);
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine("RESOURCE CLEANUP FAILED " + exception);
        result = 1;
    }
}
return result;

static async Task RunAsync(ContractContext context, string scenario, CancellationToken token)
{
    if (scenario == "datetimeoffset")
    {
        var value = new DateTimeOffset(2026, 10, 11, 8, 0, 0, TimeSpan.FromHours(8));
        var id = Guid.NewGuid();
        context.PasswordHistories.Add(new PasswordHistory { Id = id, CreatedAt = value, LockoutEnd = value.AddDays(1) });
        await context.SaveChangesAsync(token);
        context.ChangeTracker.Clear();
        var saved = await context.PasswordHistories.SingleAsync(row => row.Id == id, token);
        Console.WriteLine($"OFFSET ROUNDTRIP {saved.CreatedAt:O} LOCKOUT {saved.LockoutEnd:O}");
        Require(saved.CreatedAt.Equals(value) && saved.LockoutEnd?.Equals(value.AddDays(1)) == true, "Identity nullable and nonnullable DateTimeOffset must preserve the instant.");
        var utc = value.ToUniversalTime();
        var query = context.PasswordHistories.Where(row => row.CreatedAt == utc);
        Console.WriteLine("QUERY " + query.ToQueryString());
        Require(await query.CountAsync(token) == 1, "DateTimeOffset comparisons must compare the same instant across offsets.");
        return;
    }
    if (scenario == "idempotent-script")
    {
        var script = context.GetService<IMigrator>().GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent | MigrationsSqlGenerationOptions.NoTransactions);
        Console.WriteLine("IDEMPOTENT SCRIPT " + script);
        var statements = script.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Require(statements.Length is > 0 and <= 16, "Controlled minimal migration script must contain at most sixteen statements.");
        foreach (var statement in statements) { await context.Database.ExecuteSqlRawAsync(statement, token); }
        Console.WriteLine("FIRST SCRIPT EXECUTION PASSED");
        foreach (var statement in statements) { await context.Database.ExecuteSqlRawAsync(statement, token); }
        Require((await context.GetService<IHistoryRepository>().GetAppliedMigrationsAsync(token)).Count == 1,
            "An accepted idempotent script must safely replay and retain one history row.");
        return;
    }
    if (scenario == "history")
    {
        var history = context.GetService<IHistoryRepository>();
        await context.Database.ExecuteSqlRawAsync(history.GetCreateIfNotExistsScript(), token);
        await context.Database.ExecuteSqlRawAsync(history.GetInsertScript(new HistoryRow("202610110001_A06Probe", "10.0.12")), token);
        await context.Database.ExecuteSqlRawAsync(history.GetCreateIfNotExistsScript(), token);
        var applied = await history.GetAppliedMigrationsAsync(token);
        Require(applied.Count == 1 && applied[0].MigrationId == "202610110001_A06Probe", "Repeated history creation must retain one migration row.");
        Console.WriteLine("HISTORY ROW " + applied[0].MigrationId);
        return;
    }
    if (scenario == "binary-key")
    {
        byte[] key = [0, 1, 127, 128, 255];
        context.BinaryKeys.Add(new BinaryKey { CredentialId = key, UserId = Guid.NewGuid(), Name = "Passkey non-JSON key" });
        await context.SaveChangesAsync(token);
        context.ChangeTracker.Clear();
        var saved = await context.BinaryKeys.SingleAsync(row => row.CredentialId == key, token);
        Require(saved.CredentialId.SequenceEqual(key), "Binary key must roundtrip and compare by value.");
        return;
    }
    if (scenario == "passkey-any")
    {
        byte[] key = [0, 1, 127, 128, 255];
        var user = new IncludedUser { Id = Guid.NewGuid(), NormalizedUserName = "PASSKEY" };
        user.Passkeys.Add(new BinaryKey { CredentialId = key, UserId = user.Id, Name = "non-JSON Passkey" });
        context.IncludedUsers.Add(user);
        var other = new IncludedUser { Id = Guid.NewGuid(), NormalizedUserName = "OTHER" };
        other.Passkeys.Add(new BinaryKey { CredentialId = [9, 8, 7], UserId = other.Id, Name = "different Passkey" });
        context.IncludedUsers.Add(other);
        await context.SaveChangesAsync(token);
        context.ChangeTracker.Clear();
        byte[] lookup = [0, 1, 127, 128, 255];
        var query = context.IncludedUsers.Where(row => row.Passkeys.Any(passkey => passkey.CredentialId.SequenceEqual(lookup))).OrderBy(row => row.Id);
        Console.WriteLine("QUERY " + query.Take(1).ToQueryString());
        Require((await query.FirstOrDefaultAsync(token))?.Id == user.Id, "ABP Passkey lookup must match binary contents inside correlated Any.");
        lookup = [4, 3, 2];
        Require(await query.FirstOrDefaultAsync(token) is null, "An absent credential must not match any user.");
        return;
    }
    if (scenario == "composite-key")
    {
        var userId = Guid.NewGuid();
        context.UserTokens.AddRange(new UserToken { UserId = userId, LoginProvider = "A", Name = "refresh", Value = "one" },
            new UserToken { UserId = userId, LoginProvider = "B", Name = "refresh", Value = "two" });
        await context.SaveChangesAsync(token);
        context.ChangeTracker.Clear();
        var saved = await context.UserTokens.FindAsync([userId, "B", "refresh"], token);
        Require(saved?.Value == "two" && await context.UserTokens.CountAsync(token) == 2, "Identity three-column key must identify the correct token.");
        return;
    }
    if (scenario == "unique-null")
    {
        var tenant = Guid.NewGuid();
        context.Permissions.AddRange(
            new Permission { Id = Guid.NewGuid(), Name = "Books.Read", ProviderName = "R", ProviderKey = "admin" },
            new Permission { Id = Guid.NewGuid(), Name = "Books.Read", ProviderName = "R", ProviderKey = "admin" },
            new Permission { Id = Guid.NewGuid(), TenantId = tenant, Name = "Books.Read", ProviderName = "R", ProviderKey = "admin" });
        await context.SaveChangesAsync(token);
        Require(await context.Permissions.CountAsync(token) == 3, "Nullable composite unique index must follow the SQLite NULL reference semantics.");
        context.Permissions.Add(new Permission { Id = Guid.NewGuid(), TenantId = tenant, Name = "Books.Read", ProviderName = "R", ProviderKey = "admin" });
        await RequireRejectedAsync(() => context.SaveChangesAsync(token), "Non-null composite unique duplicate must be rejected.", "unique|duplicate|唯一");
        return;
    }
    if (scenario == "identity-includes")
    {
        var user = new IncludedUser { Id = Guid.NewGuid(), NormalizedUserName = "ADMIN" };
        user.Roles.Add(new IncludedRole { Id = Guid.NewGuid(), UserId = user.Id, Name = "admin" });
        user.Claims.Add(new IncludedClaim { Id = Guid.NewGuid(), UserId = user.Id, Name = "claim" });
        user.Logins.Add(new IncludedLogin { Id = Guid.NewGuid(), UserId = user.Id, Name = "login" });
        context.IncludedUsers.Add(user);
        await context.SaveChangesAsync(token);
        context.ChangeTracker.Clear();
        var query = context.IncludedUsers.Include(row => row.Roles).Include(row => row.Claims).Include(row => row.Logins)
            .Where(row => row.NormalizedUserName == "ADMIN").OrderBy(row => row.Id);
        Console.WriteLine("QUERY " + query.Take(1).ToQueryString());
        var saved = await query.FirstOrDefaultAsync(token);
        Require(saved is not null && saved.Roles.Count == 1 && saved.Claims.Count == 1 && saved.Logins.Count == 1,
            "Identity seed lookup must materialize all included collections after FirstOrDefault.");
        return;
    }

    var authorId = Guid.NewGuid();
    context.Authors.AddRange(
        new ProbeAuthor { Id = authorId, Name = "100% Real", BirthDate = new DateTime(1980, 1, 1) },
        new ProbeAuthor { Id = Guid.NewGuid(), Name = "100X Real", BirthDate = new DateTime(1981, 1, 1) },
        new ProbeAuthor { Id = Guid.NewGuid(), Name = "A_B", BirthDate = new DateTime(1982, 1, 1) },
        new ProbeAuthor { Id = Guid.NewGuid(), Name = "AXB", BirthDate = new DateTime(1983, 1, 1) },
        new ProbeAuthor { Id = Guid.NewGuid(), Name = "path\\name", BirthDate = new DateTime(1984, 1, 1) });
    context.Books.AddRange(
        new ProbeBook { Id = Guid.NewGuid(), AuthorId = authorId, Name = "alpha", Price = 12.5f, PublishDate = new DateTime(2026, 10, 11) },
        new ProbeBook { Id = Guid.NewGuid(), AuthorId = authorId, Name = "bravo", Price = 42f, PublishDate = new DateTime(2026, 10, 11) });
    await context.SaveChangesAsync(token);
    context.ChangeTracker.Clear();
    if (scenario == "collection-filter")
    {
        foreach (var names in new string[][] { [], ["A_B"], ["A_B", "100% Real"] })
        {
            token.ThrowIfCancellationRequested();
            var query = context.Authors.Where(author => names.Contains(author.Name));
            Console.WriteLine("QUERY " + query.ToQueryString());
            Require(await query.CountAsync(token) == names.Length, "Empty, single and multiple collection filters must match exact values.");
        }
        return;
    }
    if (scenario == "role-union")
    {
        var direct = from book in context.Books
                     join author in context.Authors on book.AuthorId equals author.Id
                     select author.Name;
        var organization = context.Authors.Where(author => author.Id == authorId).Select(author => author.Name);
        var query = direct.Union(organization);
        Console.WriteLine("QUERY " + query.ToQueryString());
        var names = await query.ToListAsync(token);
        Require(names.Count == 1 && names[0] == "100% Real", "Joined direct/organization role UNION must remove duplicates.");
        return;
    }
    if (scenario.StartsWith("contains-", StringComparison.Ordinal))
    {
        var filter = scenario switch { "contains-percent" => "%", "contains-underscore" => "_", _ => "\\" };
        var query = context.Authors.Where(author => author.Name.Contains(filter)).OrderBy(author => author.Name);
        Console.WriteLine("QUERY " + query.ToQueryString());
        var matches = await query.Select(author => author.Name).ToListAsync(token);
        var count = await query.CountAsync(token);
        Console.WriteLine("MATCHES " + string.Join(" | ", matches) + "; COUNT " + count);
        Require(matches.Count == 1 && count == 1 && matches[0].Contains(filter, StringComparison.Ordinal), "Parameterized Contains must match a literal character, not a SQL wildcard.");
        return;
    }
    if (scenario == "join-paging")
    {
        var query = from book in context.Books
                    join author in context.Authors on book.AuthorId equals author.Id
                    orderby book.Name
                    select new { book.Id, book.Name, AuthorName = author.Name, book.Price, book.PublishDate };
        Console.WriteLine("QUERY " + query.Skip(1).Take(1).ToQueryString());
        var page = await query.Skip(1).Take(1).ToListAsync(token);
        Require(page.Count == 1 && page[0].Name == "bravo" && page[0].AuthorName == "100% Real" && page[0].Price == 42f,
            "Official BookStore join, projection and ordered paging must return the correct row.");
        return;
    }
    if (scenario == "foreign-key")
    {
        context.Books.Add(new ProbeBook { Id = Guid.NewGuid(), AuthorId = Guid.NewGuid(), Name = "orphan" });
        await RequireRejectedAsync(() => context.SaveChangesAsync(token), "Book author foreign key must reject an orphan.", "foreign|外键");
        return;
    }
    if (scenario == "named-foreign-key")
    {
        await using var target = new ContractContext(context.Provider, "crud", context.DirectoryPath);
        var model = target.GetService<IDesignTimeModel>().Model;
        var commands = context.GetService<IMigrationsSqlGenerator>().Generate(
            [new DropForeignKeyOperation { Table = "A06Books", Name = "FK_A06_Book_Author" }], model);
        Require(commands.Count is > 0 and <= 16, "Named foreign-key drop must generate a bounded nonempty command list.");
        foreach (var command in commands.Take(16))
        {
            token.ThrowIfCancellationRequested();
            Console.WriteLine("MIGRATION " + command.CommandText);
            await context.Database.ExecuteSqlRawAsync(command.CommandText, token);
        }
        context.Books.Add(new ProbeBook { Id = Guid.NewGuid(), AuthorId = Guid.NewGuid(), Name = "allowed after constraint removal" });
        await context.SaveChangesAsync(token);
        return;
    }
    if (scenario == "requires-new")
    {
        await using var outer = await context.Database.BeginTransactionAsync(token);
        var status = await context.Authors.SingleAsync(author => author.Id == authorId, token);
        status.Name = "uncommitted login status";
        await context.SaveChangesAsync(token);
        await using (var inner = new ContractContext(context.Provider, scenario, context.DirectoryPath))
        {
            await using var audit = await inner.Database.BeginTransactionAsync(token);
            inner.SecurityLogs.Add(new SecurityLog { Id = Guid.NewGuid(), Message = "committed independent security log" });
            await inner.SaveChangesAsync(token);
            await audit.CommitAsync(token);
        }
        await outer.RollbackAsync(token);
        context.ChangeTracker.Clear();
        Require(await context.SecurityLogs.CountAsync(token) == 1 &&
            await context.Authors.CountAsync(author => author.Id == authorId && author.Name == "100% Real", token) == 1,
            "Independent security-log commit must survive the outer transaction rollback.");
        return;
    }
    var first = await context.Books.OrderBy(book => book.Name).FirstAsync(token);
    Require(first.Name == "alpha" && first.Price == 12.5f && first.PublishDate == new DateTime(2026, 10, 11), "Book Guid/float/DateTime must roundtrip.");
    first.Name = "updated";
    await context.SaveChangesAsync(token);
    context.ChangeTracker.Clear();
    Require(await context.Books.CountAsync(book => book.Name == "updated", token) == 1, "Book update must persist.");
}

static void Require(bool condition, string message)
{
    if (!condition) { throw new InvalidOperationException("CONTRACT FAILED: " + message); }
}

static async Task RequireRejectedAsync(Func<Task<int>> action, string message, string expectedPattern)
{
    try { await action(); }
    catch (DbUpdateException exception)
    {
        Console.WriteLine("EXPECTED CONSTRAINT REJECTION " + exception.GetBaseException().Message);
        Require(System.Text.RegularExpressions.Regex.IsMatch(exception.GetBaseException().Message, expectedPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)), "Write rejection must identify the expected constraint.");
        return;
    }
    throw new InvalidOperationException("CONTRACT FAILED: " + message);
}

internal sealed class ContractContext(string provider, string scenario, string directory) : DbContext
{
    public string Provider => provider;
    public string Scenario => scenario;
    public string DirectoryPath => directory;
    public DbSet<ProbeAuthor> Authors => Set<ProbeAuthor>();
    public DbSet<ProbeBook> Books => Set<ProbeBook>();
    public DbSet<BinaryKey> BinaryKeys => Set<BinaryKey>();
    public DbSet<UserToken> UserTokens => Set<UserToken>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<IncludedUser> IncludedUsers => Set<IncludedUser>();
    public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();
    public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.ReplaceService<IModelCacheKeyFactory, ContractModelCacheKeyFactory>();
        if (provider == "Sqlite") { options.UseSqlite("Data Source=" + Path.Combine(directory, "reference.db") + ";Pooling=False;Default Timeout=2"); }
        else { options.UseSonnetDB("Data Source=" + Path.Combine(directory, "sonnetdb")); }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ProbeAuthor>(entity =>
        {
            entity.ToTable("A06Authors"); entity.HasKey(row => row.Id); entity.Property(row => row.Id).ValueGeneratedNever();
            entity.Property(row => row.Name).IsRequired().HasMaxLength(64);
        });
        builder.Entity<ProbeBook>(entity =>
        {
            entity.ToTable("A06Books"); entity.HasKey(row => row.Id); entity.Property(row => row.Id).ValueGeneratedNever();
            entity.Property(row => row.Name).IsRequired().HasMaxLength(128);
            if (scenario is "foreign-key" or "named-foreign-key")
            {
                entity.HasOne<ProbeAuthor>().WithMany().HasForeignKey(row => row.AuthorId).OnDelete(DeleteBehavior.Cascade).HasConstraintName("FK_A06_Book_Author");
            }
        });
        if (scenario is "binary-key" or "passkey-any")
        {
            builder.Entity<BinaryKey>(entity => { entity.ToTable("A06UserPasskeys"); entity.HasKey(row => row.CredentialId); entity.Property(row => row.CredentialId).HasMaxLength(1024).ValueGeneratedNever(); });
        }
        else { builder.Ignore<BinaryKey>(); }
        if (scenario == "composite-key")
        {
            builder.Entity<UserToken>(entity => { entity.ToTable("A06UserTokens"); entity.HasKey(row => new { row.UserId, row.LoginProvider, row.Name }); });
        }
        else { builder.Ignore<UserToken>(); }
        if (scenario == "unique-null")
        {
            builder.Entity<Permission>(entity => { entity.ToTable("A06PermissionGrants"); entity.HasKey(row => row.Id); entity.HasIndex(row => new { row.TenantId, row.Name, row.ProviderName, row.ProviderKey }).IsUnique(); });
        }
        else { builder.Ignore<Permission>(); }
        if (scenario is "identity-includes" or "passkey-any")
        {
            builder.Entity<IncludedUser>(entity =>
            {
                entity.ToTable("A06Users"); entity.HasKey(row => row.Id);
                entity.HasMany(row => row.Roles).WithOne().HasForeignKey(row => row.UserId);
                entity.HasMany(row => row.Claims).WithOne().HasForeignKey(row => row.UserId);
                entity.HasMany(row => row.Logins).WithOne().HasForeignKey(row => row.UserId);
                if (scenario == "passkey-any") { entity.HasMany(row => row.Passkeys).WithOne().HasForeignKey(row => row.UserId); }
                else { entity.Ignore(row => row.Passkeys); }
            });
            builder.Entity<IncludedRole>().ToTable("A06Roles");
            builder.Entity<IncludedClaim>().ToTable("A06Claims");
            builder.Entity<IncludedLogin>().ToTable("A06Logins");
        }
        else { builder.Ignore<IncludedUser>(); builder.Ignore<IncludedRole>(); builder.Ignore<IncludedClaim>(); builder.Ignore<IncludedLogin>(); }
        if (scenario == "requires-new") { builder.Entity<SecurityLog>().ToTable("A06SecurityLogs"); }
        else { builder.Ignore<SecurityLog>(); }
        if (scenario == "datetimeoffset") { builder.Entity<PasswordHistory>().ToTable("A06UserPasswordHistories"); }
        else { builder.Ignore<PasswordHistory>(); }
    }
}

internal sealed class ContractModelCacheKeyFactory : IModelCacheKeyFactory
{
    /// <summary>按 Provider、探测场景和设计时用途隔离模型缓存。</summary>
    public object Create(DbContext context, bool designTime) => context is ContractContext contract
        ? (context.GetType(), contract.Provider, contract.Scenario, designTime)
        : (context.GetType(), designTime);
}

internal sealed class ProbeAuthor { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public DateTime BirthDate { get; set; } }
internal sealed class ProbeBook { public Guid Id { get; set; } public Guid AuthorId { get; set; } public string Name { get; set; } = string.Empty; public float Price { get; set; } public DateTime PublishDate { get; set; } }
internal sealed class BinaryKey { public byte[] CredentialId { get; set; } = []; public Guid UserId { get; set; } public string Name { get; set; } = string.Empty; }
internal sealed class UserToken { public Guid UserId { get; set; } public string LoginProvider { get; set; } = string.Empty; public string Name { get; set; } = string.Empty; public string? Value { get; set; } }
internal sealed class Permission { public Guid Id { get; set; } public Guid? TenantId { get; set; } public string Name { get; set; } = string.Empty; public string ProviderName { get; set; } = string.Empty; public string ProviderKey { get; set; } = string.Empty; }
internal sealed class IncludedUser { public Guid Id { get; set; } public string NormalizedUserName { get; set; } = string.Empty; public List<IncludedRole> Roles { get; set; } = []; public List<IncludedClaim> Claims { get; set; } = []; public List<IncludedLogin> Logins { get; set; } = []; public List<BinaryKey> Passkeys { get; set; } = []; }
internal sealed class IncludedRole { public Guid Id { get; set; } public Guid UserId { get; set; } public string Name { get; set; } = string.Empty; }
internal sealed class IncludedClaim { public Guid Id { get; set; } public Guid UserId { get; set; } public string Name { get; set; } = string.Empty; }
internal sealed class IncludedLogin { public Guid Id { get; set; } public Guid UserId { get; set; } public string Name { get; set; } = string.Empty; }
internal sealed class SecurityLog { public Guid Id { get; set; } public string Message { get; set; } = string.Empty; }
internal sealed class PasswordHistory { public Guid Id { get; set; } public DateTimeOffset CreatedAt { get; set; } public DateTimeOffset? LockoutEnd { get; set; } }

[DbContext(typeof(ContractContext))]
[Migration("202610110002_A06Minimal")]
internal sealed class MinimalMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable("A06MigrationProbe", columns: table => new { Id = table.Column<long>(nullable: false) },
            constraints: table => table.PrimaryKey("PK_A06MigrationProbe", row => row.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("A06MigrationProbe");
}
