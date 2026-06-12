using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates;
using AutoServiceAW.API.WorkshopOperations.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceAW.API.WorkshopOperations.Infrastructure.Persistence.EFC.Repositories;

public class TaskPartRepository : BaseRepository<TaskPart>, ITaskPartRepository
{
    public TaskPartRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<TaskPart>>
        FindByTaskIdAsync(int taskId)
    {
        return await Context.Set<TaskPart>()
            .Where(x => x.TaskId == taskId).ToListAsync();
    }
}