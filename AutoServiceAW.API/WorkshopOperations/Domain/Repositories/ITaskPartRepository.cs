using AutoServiceAW.API.Shared.Domain.Repositories;
using AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates;

namespace AutoServiceAW.API.WorkshopOperations.Domain.Repositories;

public interface ITaskPartRepository: IBaseRepository<TaskPart>
{
      
    Task<IEnumerable<TaskPart>> FindByTaskIdAsync(int taskId);
}