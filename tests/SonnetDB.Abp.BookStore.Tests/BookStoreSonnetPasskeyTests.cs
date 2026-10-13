using Acme.BookStore.Data;
using Acme.BookStore.DbMigrator;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Uow;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace SonnetDB.Abp.BookStore.Tests;

/// <summary>通过未修改的 ABP Identity 存储验证公开 SonnetDB 包的 Passkey JSON 合同。</summary>
public sealed class BookStoreSonnetPasskeyTests
{
    /// <summary>标准迁移后保存、重建应用读取、更新并删除包含二进制和数组的 Passkey。</summary>
    [Fact]
    public async Task PasskeyStore_WithPublicSonnetDB_PreservesValuesAcrossApplicationReconstruction()
    {
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var token = deadline.Token;
        var ownership = Guid.NewGuid().ToString("N");
        var directory = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-abp-passkey-" + ownership));
        Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(Path.Combine(directory, ".owner"), ownership, token);
        byte[] credential = Guid.NewGuid().ToByteArray();
        byte[] publicKey = [1, 2, 128, 255];
        var created = new DateTimeOffset(2026, 10, 13, 12, 0, 0, TimeSpan.FromHours(8));
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Database:Provider"] = "SonnetDB",
            ["ConnectionStrings:Default"] = "Data Source=" + Path.Combine(directory, "db")
        }).Build();

        try
        {
            await RunPhaseAsync(async services =>
            {
                await services.GetRequiredService<BookStoreDbMigrationService>().MigrateAsync().WaitAsync(token);
            });
            await RunPhaseAsync(async services =>
            {
                var store = services.GetRequiredService<IUserStore<IdentityUser>>();
                var passkeys = (IUserPasskeyStore<IdentityUser>)store;
                using var unit = services.GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true);
                var user = new IdentityUser(Guid.NewGuid(), "ef266-passkey", "ef266-passkey@example.invalid");
                Assert.True((await store.CreateAsync(user, token)).Succeeded);
                await passkeys.AddOrUpdatePasskeyAsync(user, MakePasskey(7, "初始 O'Reilly 😃"), token);
                Assert.True((await store.UpdateAsync(user, token)).Succeeded);
                await unit.CompleteAsync(token);
            });
            await RunPhaseAsync(async services =>
            {
                var store = services.GetRequiredService<IUserStore<IdentityUser>>();
                var passkeys = (IUserPasskeyStore<IdentityUser>)store;
                using var unit = services.GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true);
                var user = await passkeys.FindByPasskeyIdAsync(credential, token);
                Assert.NotNull(user);
                Assert.Equal("ef266-passkey", user.UserName);
                CheckPasskey(await passkeys.FindPasskeyAsync(user, credential, token), 7, "初始 O'Reilly 😃");
                Assert.Single(await passkeys.GetPasskeysAsync(user, token));
                await passkeys.AddOrUpdatePasskeyAsync(user, MakePasskey(uint.MaxValue, "updated 中文"), token);
                Assert.True((await store.UpdateAsync(user, token)).Succeeded);
                await unit.CompleteAsync(token);
            });
            await RunPhaseAsync(async services =>
            {
                var store = services.GetRequiredService<IUserStore<IdentityUser>>();
                var passkeys = (IUserPasskeyStore<IdentityUser>)store;
                using var unit = services.GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true);
                var user = await passkeys.FindByPasskeyIdAsync(credential, token);
                Assert.NotNull(user);
                CheckPasskey(await passkeys.FindPasskeyAsync(user, credential, token), uint.MaxValue, "updated 中文");
                await passkeys.RemovePasskeyAsync(user, credential, token);
                Assert.True((await store.UpdateAsync(user, token)).Succeeded);
                Assert.True((await store.DeleteAsync(user, token)).Succeeded);
                await unit.CompleteAsync(token);
            });
            await RunPhaseAsync(async services =>
            {
                var store = services.GetRequiredService<IUserStore<IdentityUser>>();
                using var unit = services.GetRequiredService<IUnitOfWorkManager>().Begin(requiresNew: true, isTransactional: true);
                Assert.Null(await ((IUserPasskeyStore<IdentityUser>)store).FindByPasskeyIdAsync(credential, token));
                await unit.CompleteAsync(token);
            });
        }
        finally
        {
            var expected = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-abp-passkey-" + ownership));
            if (!string.Equals(directory, expected, StringComparison.OrdinalIgnoreCase) ||
                await File.ReadAllTextAsync(Path.Combine(directory, ".owner")) != ownership)
            {
                throw new InvalidOperationException("拒绝清理归属不匹配的测试目录。");
            }
            Directory.Delete(directory, recursive: true);
        }

        async Task RunPhaseAsync(Func<IServiceProvider, Task> phase)
        {
            using var app = await AbpApplicationFactory.CreateAsync<BookStoreDbMigratorModule>(options =>
            {
                options.UseAutofac();
                options.Services.ReplaceConfiguration(configuration);
                options.AddDataMigrationEnvironment();
            });
            await app.InitializeAsync().WaitAsync(token);
            try
            {
                using var scope = app.ServiceProvider.CreateScope();
                await phase(scope.ServiceProvider).WaitAsync(token);
            }
            finally
            {
                await app.ShutdownAsync().WaitAsync(TimeSpan.FromSeconds(10));
            }
        }

        UserPasskeyInfo MakePasskey(uint count, string name) =>
            new(credential, publicKey, created, count, ["internal", "usb"], true, true, false, [3, 0, 255], [123, 125]) { Name = name };

        void CheckPasskey(UserPasskeyInfo? value, uint count, string name)
        {
            Assert.NotNull(value);
            Assert.Equal(count, value.SignCount);
            Assert.Equal(name, value.Name);
            Assert.Equal(publicKey, value.PublicKey);
            Assert.Equal(credential, value.CredentialId);
            Assert.Equal(new[] { "internal", "usb" }, value.Transports);
            Assert.Equal(created, value.CreatedAt);
            Assert.True(value.IsUserVerified);
            Assert.True(value.IsBackupEligible);
            Assert.False(value.IsBackedUp);
            Assert.Equal(new byte[] { 3, 0, 255 }, value.AttestationObject);
            Assert.Equal(new byte[] { 123, 125 }, value.ClientDataJson);
        }
    }
}
