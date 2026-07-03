using AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.InventoryManagement.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceAW.API.InventoryManagement.Interfaces.REST;

public record CreateInventoryItemResource(
    string Name,
    string Category,
    string Brand,
    decimal UnitPrice,
    int Stock,
    int MinStock,
    string? Image,
    decimal PurchasePrice = 0,
    string? QualityTier = "STANDARD",
    string? Specification = "",
    string? Presentation = "",
    string? UnitMeasure = "UNIT");

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InventoryItemsController(IInventoryItemService inventoryItemService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemResource resource)
    {
        try
        {
            var item = BuildInventoryItem(resource);
            var result = await inventoryItemService.CreateAsync(item);
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInventoryItems()
    {
        var items = await inventoryItemService.ListAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInventoryItemById(int id)
    {
        var item = await inventoryItemService.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateInventoryItem(int id, [FromBody] CreateInventoryItemResource resource)
    {
        try
        {
            var item = BuildInventoryItem(resource);
            var result = await inventoryItemService.UpdateAsync(id, item);
            return result == null ? NotFound() : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        await inventoryItemService.DeleteAsync(id);
        return NoContent();
    }

    private static InventoryItem BuildInventoryItem(CreateInventoryItemResource resource)
    {
        return new InventoryItem(
            resource.Name,
            resource.Category,
            resource.Brand,
            resource.UnitPrice,
            resource.Stock,
            resource.MinStock,
            resource.Image ?? string.Empty,
            resource.PurchasePrice,
            resource.QualityTier ?? "STANDARD",
            resource.Specification ?? string.Empty,
            resource.Presentation ?? string.Empty,
            resource.UnitMeasure ?? "UNIT");
    }
}
