using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace SonnetDB.Abp.Sample;

/// <summary>用于验证同一工作单元内第二个 DbContext 的连接与事务复用。</summary>
public sealed class DemoReadDbContext(DbContextOptions<DemoReadDbContext> options) : AbpDbContext<DemoReadDbContext>(options)
{
    /// <summary>设备集合。</summary>
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
