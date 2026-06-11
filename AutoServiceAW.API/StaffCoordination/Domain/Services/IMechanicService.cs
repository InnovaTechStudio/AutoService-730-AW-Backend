using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;

namespace AutoServiceAW.API.StaffCoordination.Domain.Services;

public interface IMechanicService
{
    Task<IEnumerable<Mechanic>> ListAsync();
    Task<Mechanic?> FindByIdAsync(int id);
    Task<Mechanic> CreateAsync(Mechanic mechanic);
    Task<Mechanic?> UpdateAsync(int id, Mechanic mechanic);
    Task<bool> DeleteAsync(int id);
}