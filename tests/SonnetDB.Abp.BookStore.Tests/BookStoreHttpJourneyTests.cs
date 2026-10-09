using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Acme.BookStore.Data;
using Acme.BookStore.DbMigrator;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Volo.Abp;
using Volo.Abp.Data;
using Xunit;

namespace SonnetDB.Abp.BookStore.Tests;

/// <summary>验证官方完整应用的真实账户、HTTP 业务和重启持久化。</summary>
public sealed class BookStoreHttpJourneyTests
{
    /// <summary>PostgreSQL 基线使用标准迁移及未替换授权的官方应用入口。</summary>
    [Fact]
    public async Task PostgreSql_OfficialApplication_MigratesLogsInPerformsCrudAndReadsAfterRestart()
    {
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
        var ownership = Guid.NewGuid().ToString("N");
        var databaseName = "sonnetdb_abp_test_" + ownership;
        var administrator = new NpgsqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable("BOOKSTORE_POSTGRES_CONNECTION") ??
            "Host=127.0.0.1;Port=54381;Database=postgres;Username=sonnetdb_abp;Password=sonnetdb_abp_demo")
        {
            Database = "postgres",
            Pooling = false,
            Timeout = 5,
            CommandTimeout = 10
        };
        var database = new NpgsqlConnectionStringBuilder(administrator.ConnectionString) { Database = databaseName };
        var createdDatabase = false;
        var directory = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-abp-bookstore-" + ownership));
        Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(Path.Combine(directory, ".owner"), ownership, deadline.Token);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Database:Provider"] = "PostgreSql",
            ["ConnectionStrings:Default"] = database.ConnectionString
        }).Build();

        try
        {
            await using (var connection = new NpgsqlConnection(administrator.ConnectionString))
            {
                await connection.OpenAsync(deadline.Token);
                await using var command = new NpgsqlCommand($"CREATE DATABASE {databaseName}", connection);
                await command.ExecuteNonQueryAsync(deadline.Token);
                createdDatabase = true;
            }
            using (var migration = await AbpApplicationFactory.CreateAsync<BookStoreDbMigratorModule>(options =>
            {
                options.UseAutofac();
                options.Services.ReplaceConfiguration(configuration);
                options.AddDataMigrationEnvironment();
            }))
            {
                await migration.InitializeAsync();
                var migrator = migration.ServiceProvider.GetRequiredService<BookStoreDbMigrationService>();
                await migrator.MigrateAsync().WaitAsync(deadline.Token);
                await migrator.MigrateAsync().WaitAsync(deadline.Token);
                await migration.ShutdownAsync();
            }

            string authorId;
            string bookId;
            await using (var first = new OfficialApplication(configuration))
            using (var client = first.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost:44339") }))
            {
                await LoginAsync(client, deadline.Token);
                var authors = await ReadJsonAsync(client, "/api/app/author?MaxResultCount=10", deadline.Token);
                Assert.True(authors["totalCount"]!.GetValue<int>() >= 2);
                await AssertPageAsync(client, "/Authors", deadline.Token);
                await AssertPageAsync(client, "/Books", deadline.Token);
                await SetAntiforgeryAsync(client, deadline.Token);

                var author = new JsonObject
                {
                    ["name"] = "A06 HTTP author",
                    ["birthDate"] = "1980-01-01T00:00:00",
                    ["shortBio"] = "PostgreSQL baseline through unchanged ABP services"
                };
                var createdAuthor = await SendJsonAsync(client, HttpMethod.Post, "/api/app/author", author, deadline.Token);
                authorId = createdAuthor!["id"]!.GetValue<string>();
                var book = new JsonObject
                {
                    ["name"] = "A06 HTTP book",
                    ["type"] = 1,
                    ["publishDate"] = "2026-10-09T00:00:00",
                    ["price"] = 12.5,
                    ["authorId"] = authorId
                };
                var createdBook = await SendJsonAsync(client, HttpMethod.Post, "/api/app/book", book, deadline.Token);
                bookId = createdBook!["id"]!.GetValue<string>();
                book["name"] = "A06 HTTP book updated";
                await SendJsonAsync(client, HttpMethod.Put, $"/api/app/book/{bookId}", book, deadline.Token);
                author["shortBio"] = "Updated through the official author application service";
                await SendJsonAsync(client, HttpMethod.Put, $"/api/app/author/{authorId}", author, deadline.Token);
                var page = await ReadJsonAsync(client, "/api/app/book?Sorting=name&SkipCount=0&MaxResultCount=1", deadline.Token);
                Assert.Single(page["items"]!.AsArray());
                Assert.True(page["totalCount"]!.GetValue<int>() >= 3);
            }

            await using (var restarted = new OfficialApplication(configuration))
            using (var client = restarted.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost:44339") }))
            {
                await LoginAsync(client, deadline.Token);
                var book = await ReadJsonAsync(client, $"/api/app/book/{bookId}", deadline.Token);
                Assert.Equal("A06 HTTP book updated", book["name"]!.GetValue<string>());
                Assert.Equal("A06 HTTP author", book["authorName"]!.GetValue<string>());
                var author = await ReadJsonAsync(client, $"/api/app/author/{authorId}", deadline.Token);
                Assert.Equal("Updated through the official author application service", author["shortBio"]!.GetValue<string>());
                await SetAntiforgeryAsync(client, deadline.Token);
                using var deleteBook = await client.DeleteAsync($"/api/app/book/{bookId}", deadline.Token);
                deleteBook.EnsureSuccessStatusCode();
                using var deleteAuthor = await client.DeleteAsync($"/api/app/author/{authorId}", deadline.Token);
                deleteAuthor.EnsureSuccessStatusCode();
                var remaining = await ReadJsonAsync(client, "/api/app/author?Filter=A06%20HTTP%20author", deadline.Token);
                Assert.Equal(0, remaining["totalCount"]!.GetValue<int>());
            }
        }
        finally
        {
            if (createdDatabase)
            {
                if (databaseName != "sonnetdb_abp_test_" + ownership || !Regex.IsMatch(databaseName, "^sonnetdb_abp_test_[a-f0-9]{32}$", RegexOptions.None, TimeSpan.FromSeconds(1)))
                {
                    throw new InvalidOperationException("Refusing to remove a database without matching task ownership.");
                }
                using var cleanupDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await using var connection = new NpgsqlConnection(administrator.ConnectionString);
                await connection.OpenAsync(cleanupDeadline.Token);
                await using var command = new NpgsqlCommand($"DROP DATABASE {databaseName} WITH (FORCE)", connection);
                await command.ExecuteNonQueryAsync(cleanupDeadline.Token);
            }
            var ownedRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "sonnetdb-abp-bookstore-" + ownership));
            if (!string.Equals(directory, ownedRoot, StringComparison.OrdinalIgnoreCase) ||
                await File.ReadAllTextAsync(Path.Combine(directory, ".owner")) != ownership)
            {
                throw new InvalidOperationException("Refusing to remove a directory without matching task ownership.");
            }
            Directory.Delete(directory, recursive: true);
        }
    }

    private static async Task LoginAsync(HttpClient client, CancellationToken token)
    {
        using var login = await client.GetAsync("/Account/Login", token);
        login.EnsureSuccessStatusCode();
        var html = await login.Content.ReadAsStringAsync(token);
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = GetFormToken(html),
            ["LoginInput.UserNameOrEmailAddress"] = "admin",
            ["LoginInput.Password"] = "1q2w3E*",
            ["LoginInput.RememberMe"] = "false"
        });
        using var response = await client.PostAsync("/Account/Login", form, token);
        response.EnsureSuccessStatusCode();
        var configuration = await ReadJsonAsync(client, "/api/abp/application-configuration", token);
        Assert.True(configuration["currentUser"]!["isAuthenticated"]!.GetValue<bool>());
        Assert.Equal("admin", configuration["currentUser"]!["userName"]!.GetValue<string>());
    }

    private static async Task SetAntiforgeryAsync(HttpClient client, CancellationToken token)
    {
        using var response = await client.GetAsync("/Books/CreateModal", token);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync(token);
        client.DefaultRequestHeaders.Remove("RequestVerificationToken");
        client.DefaultRequestHeaders.Add("RequestVerificationToken", GetFormToken(html));
    }

    private static string GetFormToken(string html)
    {
        var match = Regex.Match(html, "<input[^>]*name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"", RegexOptions.None, TimeSpan.FromSeconds(1));
        Assert.True(match.Success, "The official Razor form must contain its antiforgery token.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    private static async Task AssertPageAsync(HttpClient client, string uri, CancellationToken token)
    {
        using var response = await client.GetAsync(uri, token);
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    private static async Task<JsonNode> ReadJsonAsync(HttpClient client, string uri, CancellationToken token)
    {
        using var response = await client.GetAsync(uri, token);
        response.EnsureSuccessStatusCode();
        return JsonNode.Parse(await response.Content.ReadAsStringAsync(token))!;
    }

    private static async Task<JsonNode?> SendJsonAsync(HttpClient client, HttpMethod method, string uri, JsonObject body, CancellationToken token)
    {
        using var request = new HttpRequestMessage(method, uri)
        {
            Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json")
        };
        using var response = await client.SendAsync(request, token);
        response.EnsureSuccessStatusCode();
        var text = await response.Content.ReadAsStringAsync(token);
        return string.IsNullOrWhiteSpace(text) ? null : JsonNode.Parse(text);
    }

    private sealed class OfficialApplication(IConfiguration configuration) : WebApplicationFactory<Acme.BookStore.Web.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var webRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../samples/BookStore/src/Acme.BookStore.Web"));
            builder.UseContentRoot(webRoot).UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, options) => options.AddConfiguration(configuration));
        }
    }
}
