using AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.InventoryManagement.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceAW.API.InventoryManagement.Interfaces.REST;
/// <summary>
/// Data Transfer Object (DTO) for creating or updating an inventory item resource.
/// </summary>
/// <param name="Name">The descriptive name of the item.</param>
/// <param name="Category">The category classification of the item.</param>
/// <param name="Brand">The manufacturer or brand of the item.</param>
/// <param name="UnitPrice">The standard commercial price per unit.</param>
/// <param name="Stock">The physical quantity available in stock.</param>
/// <param name="MinStock">The minimum required safety stock limit.</param>
public record CreateInventoryItemResource(string Name, string Category, string Brand, decimal UnitPrice, int Stock, int MinStock);

/// <summary>
/// Exposes RESTful endpoints for managing warehouse inventory items.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InventoryItemsController(IInventoryItemService inventoryItemService) : ControllerBase
{
     #region Methods

    /// <summary>
    /// Registers a new inventory item into the warehouse management context.
    /// </summary>
    /// <param name="resource">The inventory item data transfer object containing catalog details.</param>
    /// <returns>An <see cref="IActionResult"/> with a 201 Created status code and the created item data.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateInventoryItem([FromBody] CreateInventoryItemResource resource)
    {
        var item = new InventoryItem(resource.Name, resource.Category, resource.Brand, resource.UnitPrice, resource.Stock, resource.MinStock);
        var result = await inventoryItemService.CreateAsync(item);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Retrieves all inventory items registered in the global catalog.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> containing the list of inventory items.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllInventoryItems()
    {
        var items = await inventoryItemService.ListAsync();
        return Ok(items);
    }
    
    /// <summary>
    /// Retrieves a specific inventory item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory item.</param>
    /// <returns>An <see cref="IActionResult"/> with 200 OK and the item data, or 404 Not Found if it does not exist.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInventoryItemById(int id)
    {
        var item = await inventoryItemService.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Updates an existing inventory item's specifications and stock constraints.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory item to update.</param>
    /// <param name="resource">The data transfer object containing the updated parameters.</param>
    /// <returns>An <see cref="IActionResult"/> with 200 OK and the updated item data, or 404 Not Found if the operation failed.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventoryItem(int id, [FromBody] CreateInventoryItemResource resource)
    {
        var item = new InventoryItem(resource.Name, resource.Category, resource.Brand, resource.UnitPrice, resource.Stock, resource.MinStock);
        var result = await inventoryItemService.UpdateAsync(id, item);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Deletes an inventory item from the catalog data storage by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory item to remove.</param>
    /// <returns>An <see cref="IActionResult"/> with 204 No Content status code.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventoryItem(int id)
    {
        await inventoryItemService.DeleteAsync(id);
        return NoContent();
    }

    #endregion
    
}