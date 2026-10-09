using Acme.BookStore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Acme.BookStore.DbMigrator;

/// <summary>供标准 EF 工具在启动程序中发现实际应用上下文。</summary>
public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<BookStoreDbContext>
{
    /// <summary>共用官方上下文的配置工厂，选择本次生成迁移的 Provider。</summary>
    public BookStoreDbContext CreateDbContext(string[] args) => new BookStoreDbContextFactory().CreateDbContext(args);
}
