using AutoServiceAW.API.Shared.Domain.Repositories;
using Task = AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates.Task;
namespace AutoServiceAW.API.WorkshopOperations.Domain.Repositories;

/// <summary>
/// Defines the repository contract for managing <see cref="Task"/> entities.
/// Inherits basic CRUD operations from <see cref="IBaseRepository{Task}"/>.
/// </summary>
public interface ITaskRepository : IBaseRepository<Task>
{
    #region Methods

    /// <summary>
    /// Retrieves all tasks linked to a specific work order identifier sequence asynchronously.
    /// </summary>
    /// <param name="workOrderId">The unique identifier of the target work order.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of matching <see cref="Task"/> entities.</returns>
    Task<IEnumerable<Task>> FindByWorkOrderIdAsync(int workOrderId);

    /// <summary>
    /// Retrieves all tasks assigned to a specific mechanic identifier sequence asynchronously.
    /// </summary>
    /// <param name="mechanicId">The unique identifier of the target mechanic.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of matching <see cref="Task"/> entities.</returns>
    Task<IEnumerable<Task>> FindByMechanicIdAsync(int mechanicId);

    #endregion
}