using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using AutoServiceAW.API.WorkshopOperations.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Task = AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates.Task;

namespace AutoServiceAW.API.WorkshopOperations.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Infrastructure implementation of the <see cref="ITaskRepository"/> contract
/// using Entity Framework Core and inheriting generic operations from <see cref="BaseRepository{Task}"/>.
/// </summary>
public class TaskRepository(AppDbContext context) : BaseRepository<Task>(context), ITaskRepository
{
    #region Methods

    /// <summary>
    /// Queries the data persistence context to find all task records associated with a specific parent work order.
    /// </summary>
    /// <param name="workOrderId">The parent work order unique numerical key.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the matching list of tasks.</returns>
    public async Task<IEnumerable<Task>> FindByWorkOrderIdAsync(int workOrderId)
    {
        return await Context.Tasks.Where(t => t.WorkOrderId == workOrderId).ToListAsync();
    }

    /// <summary>
    /// Queries the data persistence context to find all task records assigned to a specific mechanic.
    /// </summary>
    /// <param name="mechanicId">The target mechanic unique numerical tracking key.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the matching list of tasks assigned to the technician.</returns>
    public async Task<IEnumerable<Task>> FindByMechanicIdAsync(int mechanicId)
    {
        return await Context.Tasks.Where(t => t.MechanicId == mechanicId).ToListAsync();
    }

    #endregion
}