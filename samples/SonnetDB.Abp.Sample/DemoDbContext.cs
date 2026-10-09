using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SonnetDB.Abp.Sample;

/// <summary>使用 ABP 原生数据过滤和实体约定的样例上下文。</summary>
public sealed class DemoDbContext(DbContextOptions<DemoDbContext> options) : AbpDbContext<DemoDbContext>(options)
{
    /// <summary>样例设备集合。</summary>
    public DbSet<DemoDevice> Devices => Set<DemoDevice>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DemoDevice>(entity =>
        {
            entity.ToTable("DemoDevices");
            entity.ConfigureByConvention();
            entity.Property(device => device.Name).HasMaxLength(128).IsRequired();
        });
    }
}
