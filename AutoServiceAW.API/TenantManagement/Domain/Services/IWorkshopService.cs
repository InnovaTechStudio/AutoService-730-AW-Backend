using AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;

namespace AutoServiceAW.API.TenantManagement.Domain.Services;

public interface IWorkshopService
{
    Task<IEnumerable<Workshop>> ListAsync();
    Task<Workshop?> FindByIdAsync(int id);
    Task<Workshop> CreateAsync(Workshop workshop);
    Task<Workshop?> UpdateAsync(int id, Workshop workshop);
    Task<bool> DeleteAsync(int id);
}