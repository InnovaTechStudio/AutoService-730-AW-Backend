using AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.TenantManagement.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceAW.API.TenantManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class WorkshopsController(IWorkshopService workshopService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var workshops = await workshopService.ListAsync();
        return Ok(workshops);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var workshop = await workshopService.FindByIdAsync(id);

        if (workshop is null)
            return NotFound();

        return Ok(workshop);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Workshop workshop)
    {
        var createdWorkshop = await workshopService.CreateAsync(workshop);

        return Created($"/api/v1/workshops/{createdWorkshop.Id}", createdWorkshop);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Workshop workshop)
    {
        var updatedWorkshop = await workshopService.UpdateAsync(id, workshop);

        if (updatedWorkshop is null)
            return NotFound();

        return Ok(updatedWorkshop);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await workshopService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}