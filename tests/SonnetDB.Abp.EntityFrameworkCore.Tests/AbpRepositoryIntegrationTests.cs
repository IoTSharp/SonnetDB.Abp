using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SonnetDB.Abp.Sample;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace SonnetDB.Abp.EntityFrameworkCore.Tests;

public sealed class AbpRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly string _ownership = Guid.NewGuid().ToString("N");
    private string _directory = string.Empty;
    private IAbpApplicationWithInternalServiceProvider? _application;
    private IServiceScope? _scope;

    private IServiceProvider Services => _scope!.ServiceProvider;
    private IUnitOfWorkManager UnitOfWorkManager => Services.GetRequiredService<IUnitOfWorkManager>();
    private IRepository<DemoDevice, Guid> Repository => Services.GetRequiredService<IRepository<DemoDevice, Guid>>();

    public async Task InitializeAsync()
    {
        _directory = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-abp-tests-" + _ownership));
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, ".owner"), _ownership);
        try
        {
            _application = await AbpApplicationFactory.CreateAsync<DemoModule>(options =>
            {
                options.UseAutofac();
                options.Services.Configure<AbpDbConnectionOptions>(connectionOptions =>
                    connectionOptions.ConnectionStrings.Default = $"Data Source={Path.Combine(_directory, "database")}");
            });
            await _application.InitializeAsync();
            _scope = _application.ServiceProvider.CreateScope();

            using var initialization = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
            var context = await Services.GetRequiredService<IDbContextProvider<DemoDbContext>>().GetDbContextAsync();
            Assert.Equal("SonnetDB.EntityFrameworkCore", context.Database.ProviderName);
            await context.Database.EnsureCreatedAsync();
            await initialization.CompleteAsync();
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    [Fact]
    public async Task Repository_CreateReadUpdate_PersistsAuditedEntity()
    {
        var id = Guid.NewGuid();
        await InsertAsync(new DemoDevice(id, "pump"));

        using (var updating = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            var device = await Repository.GetAsync(id);
            Assert.NotEqual(default, device.CreationTime);
            Assert.False(string.IsNullOrWhiteSpace(device.ConcurrencyStamp));
            device.Rename("updated-pump");
            await Repository.UpdateAsync(device, autoSave: true);
            await updating.CompleteAsync();
        }

        using var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        var saved = await Repository.GetAsync(id);
        Assert.Equal("updated-pump", saved.Name);
        Assert.NotNull(saved.LastModificationTime);
        await reading.CompleteAsync();
    }

    [Fact]
    public async Task UnitOfWork_AfterAutoSaveRollback_DiscardsChanges()
    {
        var id = Guid.NewGuid();
        using (var writing = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            await Repository.InsertAsync(new DemoDevice(id, "rollback"), autoSave: true);
            await writing.RollbackAsync();
        }

        using var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        Assert.Null(await Repository.FindAsync(id));
        await reading.CompleteAsync();
    }

    [Fact]
    public async Task UnitOfWork_DisposedWithoutComplete_DiscardsChanges()
    {
        var id = Guid.NewGuid();
        using (UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            await Repository.InsertAsync(new DemoDevice(id, "not-completed"), autoSave: true);
        }

        using var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        Assert.Null(await Repository.FindAsync(id));
        await reading.CompleteAsync();
    }

    [Fact]
    public async Task Repository_SoftDeleteAndFilterToggle_PreservesDeletedRow()
    {
        var id = Guid.NewGuid();
        await InsertAsync(new DemoDevice(id, "deleted"));
        using (var deleting = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            await Repository.DeleteAsync(id, autoSave: true);
            await deleting.CompleteAsync();
        }

        using var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        Assert.Null(await Repository.FindAsync(id));
        using (Services.GetRequiredService<IDataFilter>().Disable<ISoftDelete>())
        {
            var deleted = await Repository.GetAsync(id);
            Assert.True(deleted.IsDeleted);
            Assert.NotNull(deleted.DeletionTime);
        }
        Assert.Null(await Repository.FindAsync(id));
        await reading.CompleteAsync();
    }

    [Fact]
    public async Task Repository_TenantSwitch_IsolatesHostAndTenants()
    {
        var tenant = Services.GetRequiredService<ICurrentTenant>();
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var idA = Guid.NewGuid();
        var idB = Guid.NewGuid();
        using (tenant.Change(tenantA))
        {
            await InsertAsync(new DemoDevice(idA, "tenant-a", tenantA));
        }
        using (tenant.Change(tenantB))
        {
            await InsertAsync(new DemoDevice(idB, "tenant-b", tenantB));
        }

        using (var hostReading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            Assert.Equal(0, await Repository.GetCountAsync());
            await hostReading.CompleteAsync();
        }
        using (tenant.Change(tenantA))
        using (var tenantReading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            Assert.Equal("tenant-a", (await Repository.GetAsync(idA)).Name);
            Assert.Null(await Repository.FindAsync(idB));
            Assert.Equal(1, await Repository.GetCountAsync());
            await tenantReading.CompleteAsync();
        }
        using (tenant.Change(tenantB))
        using (var tenantReading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            Assert.Equal("tenant-b", (await Repository.GetAsync(idB)).Name);
            Assert.Null(await Repository.FindAsync(idA));
            await tenantReading.CompleteAsync();
        }
        using (Services.GetRequiredService<IDataFilter>().Disable<IMultiTenant>())
        using (var allTenants = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            Assert.Equal(2, await Repository.GetCountAsync());
            await allTenants.CompleteAsync();
        }
    }

    [Fact]
    public async Task Repository_StaleConcurrencyStamp_RejectsUpdate()
    {
        var id = Guid.NewGuid();
        await InsertAsync(new DemoDevice(id, "original"));
        DemoDevice stale;
        using (var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
        {
            stale = await Repository.GetAsync(id);
            await reading.CompleteAsync();
        }
        using (var updating = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            var current = await Repository.GetAsync(id);
            current.Rename("winner");
            await Repository.UpdateAsync(current, autoSave: true);
            await updating.CompleteAsync();
        }

        using (var conflicting = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            stale.Rename("stale");
            await Assert.ThrowsAsync<AbpDbConcurrencyException>(() => Repository.UpdateAsync(stale, autoSave: true));
            await conflicting.RollbackAsync();
        }
        using var verification = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        Assert.Equal("winner", (await Repository.GetAsync(id)).Name);
        await verification.CompleteAsync();
    }

    [Fact]
    public async Task UnitOfWork_TwoDbContexts_SharesConnectionAndRollsBackBoth()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        using (var writing = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {
            var first = await Services.GetRequiredService<IDbContextProvider<DemoDbContext>>().GetDbContextAsync();
            var second = await Services.GetRequiredService<IDbContextProvider<DemoReadDbContext>>().GetDbContextAsync();
            Assert.Same(first.Database.GetDbConnection(), second.Database.GetDbConnection());
            Assert.NotNull(first.Database.CurrentTransaction);
            Assert.NotNull(second.Database.CurrentTransaction);
            Assert.Same(first.Database.CurrentTransaction.GetDbTransaction(), second.Database.CurrentTransaction.GetDbTransaction());

            first.Devices.Add(new DemoDevice(firstId, "first-context"));
            await first.SaveChangesAsync();
            second.Devices.Add(new DemoDevice(secondId, "second-context"));
            await second.SaveChangesAsync();
            await writing.RollbackAsync();
        }

        using var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        Assert.Null(await Repository.FindAsync(firstId));
        Assert.Null(await Repository.FindAsync(secondId));
        await reading.CompleteAsync();
    }

    [Fact]
    public async Task Repository_OrderedPaging_ReturnsRequestedSlice()
    {
        await InsertAsync(new DemoDevice(Guid.NewGuid(), "alpha"));
        await InsertAsync(new DemoDevice(Guid.NewGuid(), "bravo"));
        await InsertAsync(new DemoDevice(Guid.NewGuid(), "charlie"));
        using var reading = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        var query = await Repository.GetQueryableAsync();
        var page = await query.OrderBy(device => device.Name).Skip(1).Take(1).ToListAsync();
        Assert.Equal("bravo", Assert.Single(page).Name);
        await reading.CompleteAsync();
    }

    private async Task InsertAsync(DemoDevice device)
    {
        using var writing = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
        await Repository.InsertAsync(device, autoSave: true);
        await writing.CompleteAsync();
    }

    public async Task DisposeAsync()
    {
        try
        {
            _scope?.Dispose();
            _scope = null;
            if (_application is not null)
            {
                await _application.ShutdownAsync();
                _application.Dispose();
                _application = null;
            }
        }
        finally
        {
            var expected = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-abp-tests-" + _ownership));
            if (_directory.Length > 0 && Directory.Exists(_directory))
            {
                Assert.Equal(expected, Path.GetFullPath(_directory), StringComparer.OrdinalIgnoreCase);
                Assert.Equal(_ownership, await File.ReadAllTextAsync(Path.Combine(_directory, ".owner")));
                Assert.False(File.GetAttributes(_directory).HasFlag(FileAttributes.ReparsePoint));
                Directory.Delete(_directory, recursive: true);
            }
        }
    }
}
