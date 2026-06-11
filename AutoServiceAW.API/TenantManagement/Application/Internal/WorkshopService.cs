using AutoServiceAW.API.Shared.Domain.Repositories;
using AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.TenantManagement.Domain.Repositories;
using AutoServiceAW.API.TenantManagement.Domain.Services;

namespace AutoServiceAW.API.TenantManagement.Application.Internal;

public class WorkshopService(
    IWorkshopRepository workshopRepository,
    IUnitOfWork unitOfWork) : IWorkshopService
{
    public async Task<IEnumerable<Workshop>> ListAsync()
    {
        return await workshopRepository.ListAsync();
    }

    public async Task<Workshop?> FindByIdAsync(int id)
    {
        return await workshopRepository.FindByIdAsync(id);
    }

    public async Task<Workshop> CreateAsync(Workshop workshop)
    {
        await workshopRepository.AddAsync(workshop);
        await unitOfWork.CompleteAsync();

        return workshop;
    }

    public async Task<Workshop?> UpdateAsync(int id, Workshop workshop)
    {
        var existingWorkshop = await workshopRepository.FindByIdAsync(id);

        if (existingWorkshop is null)
            return null;

        existingWorkshop.Name = workshop.Name;
        existingWorkshop.TenantId = workshop.TenantId;

        workshopRepository.Update(existingWorkshop);
        await unitOfWork.CompleteAsync();

        return existingWorkshop;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingWorkshop = await workshopRepository.FindByIdAsync(id);

        if (existingWorkshop is null)
            return false;

        workshopRepository.Remove(existingWorkshop);
        await unitOfWork.CompleteAsync();

        return true;
    }
}