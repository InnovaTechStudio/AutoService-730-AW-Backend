namespace AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;

public class Workshop
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TenantId { get; set; }

    public Workshop()
    {
        Name = string.Empty;
        TenantId = string.Empty;
    }

    public Workshop(string name, string tenantId)
    {
        Name = name;
        TenantId = tenantId;
    }
}