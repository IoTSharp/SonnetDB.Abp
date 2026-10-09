using Microsoft.Extensions.DependencyInjection;
using SonnetDB.Abp.Sample;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;

var databaseDirectory = Path.GetFullPath(args.FirstOrDefault() ?? "demo-data");
using var application = await AbpApplicationFactory.CreateAsync<DemoModule>(options =>
{
    options.UseAutofac();
    options.Services.Configure<AbpDbConnectionOptions>(connectionOptions =>
        connectionOptions.ConnectionStrings.Default = $"Data Source={databaseDirectory}");
});
await application.InitializeAsync();
try
{
    using var scope = application.ServiceProvider.CreateScope();
    var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
    var dbContextProvider = scope.ServiceProvider.GetRequiredService<IDbContextProvider<DemoDbContext>>();
    using (var initialization = unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
    {
        var context = await dbContextProvider.GetDbContextAsync();
        await context.Database.EnsureCreatedAsync();
        await initialization.CompleteAsync();
    }

    var repository = scope.ServiceProvider.GetRequiredService<IRepository<DemoDevice, Guid>>();
    var deviceId = Guid.NewGuid();
    using (var writing = unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
    {
        await repository.InsertAsync(new DemoDevice(deviceId, "ABP + SonnetDB"), autoSave: true);
        await writing.CompleteAsync();
    }

    using (var reading = unitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
    {
        var device = await repository.GetAsync(deviceId);
        Console.WriteLine($"Persisted via ABP IRepository: {device.Name} ({device.Id})");
        await reading.CompleteAsync();
    }
}
finally
{
    await application.ShutdownAsync();
}
