using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace SonnetDB.Abp.Sample;

/// <summary>用于验证仓储、租户隔离、软删除和审计的样例设备。</summary>
public sealed class DemoDevice : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>设备名称。</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>设备所属租户；空值表示宿主。</summary>
    public Guid? TenantId { get; private set; }

    private DemoDevice()
    {
    }

    /// <summary>创建样例设备。</summary>
    /// <param name="id">设备标识。</param>
    /// <param name="name">设备名称。</param>
    /// <param name="tenantId">所属租户。</param>
    public DemoDevice(Guid id, string name, Guid? tenantId = null) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        TenantId = tenantId;
    }

    /// <summary>修改设备名称。</summary>
    /// <param name="name">新名称。</param>
    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }
}
