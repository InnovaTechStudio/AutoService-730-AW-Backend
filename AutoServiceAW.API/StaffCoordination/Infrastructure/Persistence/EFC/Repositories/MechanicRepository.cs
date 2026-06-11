using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;
using AutoServiceAW.API.StaffCoordination.Domain.Repositories;

namespace AutoServiceAW.API.StaffCoordination.Infrastructure.Persistence.EFC.Repositories;

public class MechanicRepository(AppDbContext context)
    : BaseRepository<Mechanic>(context), IMechanicRepository
{
}