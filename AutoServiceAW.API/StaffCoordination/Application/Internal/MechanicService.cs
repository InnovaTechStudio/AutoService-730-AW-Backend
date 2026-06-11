using AutoServiceAW.API.Shared.Domain.Repositories;
using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;
using AutoServiceAW.API.StaffCoordination.Domain.Repositories;
using AutoServiceAW.API.StaffCoordination.Domain.Services;

namespace AutoServiceAW.API.StaffCoordination.Application.Internal;

public class MechanicService(
    IMechanicRepository mechanicRepository,
    IUnitOfWork unitOfWork) : IMechanicService
{
    public async Task<IEnumerable<Mechanic>> ListAsync()
    {
        return await mechanicRepository.ListAsync();
    }

    public async Task<Mechanic?> FindByIdAsync(int id)
    {
        return await mechanicRepository.FindByIdAsync(id);
    }

    public async Task<Mechanic> CreateAsync(Mechanic mechanic)
    {
        await mechanicRepository.AddAsync(mechanic);
        await unitOfWork.CompleteAsync();

        return mechanic;
    }

    public async Task<Mechanic?> UpdateAsync(int id, Mechanic mechanic)
    {
        var existingMechanic = await mechanicRepository.FindByIdAsync(id);

        if (existingMechanic is null)
            return null;

        existingMechanic.FullName = mechanic.FullName;
        existingMechanic.Specialty = mechanic.Specialty;
        existingMechanic.MaxCapacity = mechanic.MaxCapacity;
        existingMechanic.Email = mechanic.Email;
        existingMechanic.Password = mechanic.Password;

        mechanicRepository.Update(existingMechanic);
        await unitOfWork.CompleteAsync();

        return existingMechanic;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingMechanic = await mechanicRepository.FindByIdAsync(id);

        if (existingMechanic is null)
            return false;

        mechanicRepository.Remove(existingMechanic);
        await unitOfWork.CompleteAsync();

        return true;
    }
}