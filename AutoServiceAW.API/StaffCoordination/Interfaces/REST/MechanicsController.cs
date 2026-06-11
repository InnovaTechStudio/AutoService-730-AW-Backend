using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;
using AutoServiceAW.API.StaffCoordination.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceAW.API.StaffCoordination.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class MechanicsController(IMechanicService mechanicService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var mechanics = await mechanicService.ListAsync();
        return Ok(mechanics);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var mechanic = await mechanicService.FindByIdAsync(id);

        if (mechanic is null)
            return NotFound();

        return Ok(mechanic);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Mechanic mechanic)
    {
        var createdMechanic = await mechanicService.CreateAsync(mechanic);

        return Created($"/api/v1/mechanics/{createdMechanic.Id}", createdMechanic);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Mechanic mechanic)
    {
        var updatedMechanic = await mechanicService.UpdateAsync(id, mechanic);

        if (updatedMechanic is null)
            return NotFound();

        return Ok(updatedMechanic);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await mechanicService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}