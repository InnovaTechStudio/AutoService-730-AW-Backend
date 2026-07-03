using AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.InventoryManagement.Domain.Services;
using AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates;
using AutoServiceAW.API.WorkshopOperations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates.Task;

namespace AutoServiceAW.API.WorkshopOperations.Interfaces.REST;

public record CreateTaskPartResource(int InventoryItemId, int Quantity);

public record CreateTaskResource(
    int WorkOrderId,
    int? MechanicId,
    string Description,
    string Priority,
    int EstimatedTime,
    decimal LaborPrice,
    List<CreateTaskPartResource> Parts,
    decimal LaborCost = 0);

public record UpdateTaskResource(
    string Description,
    string Status,
    string Priority,
    int EstimatedTime,
    decimal LaborPrice,
    int? MechanicId,
    decimal LaborCost = 0);

public record PatchTaskResource(
    string? Status,
    string? TechnicalDiagnosis,
    string? CustomerExplanation,
    string? InternalObservation,
    string? EvidenceRegistered,
    string? AdminReviewStatus);

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TasksController(
    ITaskService taskService,
    IWorkOrderService workOrderService,
    IInventoryItemService inventoryItemService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskResource resource)
    {
        var requestedParts = resource.Parts ?? [];
        var selectedItems = new List<(CreateTaskPartResource Request, InventoryItem Item)>();

        foreach (var part in requestedParts)
        {
            if (part.Quantity <= 0)
                return BadRequest(new { message = "Part quantity must be greater than zero" });

            var inventoryItem = await inventoryItemService.GetByIdAsync(part.InventoryItemId);
            if (inventoryItem == null)
                return BadRequest(new { message = $"Inventory item {part.InventoryItemId} was not found" });

            selectedItems.Add((part, inventoryItem));
        }

        foreach (var itemGroup in selectedItems.GroupBy(selection => selection.Item.Id))
        {
            var item = itemGroup.First().Item;
            var totalRequested = itemGroup.Sum(selection => selection.Request.Quantity);

            if (item.Stock < totalRequested)
            {
                return BadRequest(new
                {
                    message = $"Insufficient stock for {item.Name}",
                    available = item.Stock,
                    requested = totalRequested
                });
            }
        }

        var task = new Task(
            resource.WorkOrderId,
            resource.MechanicId,
            resource.Description,
            "PENDING",
            resource.Priority,
            resource.EstimatedTime,
            resource.LaborPrice,
            resource.LaborCost);

        decimal materialsSaleTotal = 0;
        decimal materialsPurchaseTotal = 0;

        foreach (var selection in selectedItems)
        {
            var request = selection.Request;
            var inventoryItem = selection.Item;

            task.AddPart(new TaskPart(
                0,
                inventoryItem.Id,
                inventoryItem.Name,
                request.Quantity,
                inventoryItem.UnitPrice,
                inventoryItem.PurchasePrice,
                inventoryItem.Brand,
                inventoryItem.QualityTier));

            materialsSaleTotal += inventoryItem.UnitPrice * request.Quantity;
            materialsPurchaseTotal += inventoryItem.PurchasePrice * request.Quantity;
        }

        task.UpdateMaterialsCost(materialsSaleTotal, materialsPurchaseTotal);

        foreach (var itemGroup in selectedItems.GroupBy(selection => selection.Item.Id))
        {
            var inventoryItem = itemGroup.First().Item;
            var totalRequested = itemGroup.Sum(selection => selection.Request.Quantity);
            inventoryItem.ConsumeStock(totalRequested);
            await inventoryItemService.UpdateAsync(inventoryItem.Id, inventoryItem);
        }

        var result = await taskService.CreateAsync(task);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] int? workOrderId, [FromQuery] int? mechanicId)
    {
        var workshopId = User.Claims.FirstOrDefault(c => c.Type == "WorkshopId")?.Value;
        if (string.IsNullOrEmpty(workshopId)) return Unauthorized();

        IEnumerable<Task> tasks;

        if (workOrderId.HasValue)
        {
            tasks = await taskService.ListByWorkOrderIdAsync(workOrderId.Value);
        }
        else if (mechanicId.HasValue)
        {
            tasks = await taskService.ListByMechanicIdAsync(mechanicId.Value);
        }
        else
        {
            var allTasks = await taskService.ListAsync();
            var workOrders = await workOrderService.ListAsync();
            var workshopOrderIds = workOrders
                .Where(workOrder => workOrder.WorkshopId == workshopId)
                .Select(workOrder => workOrder.Id)
                .ToHashSet();

            tasks = allTasks.Where(task => workshopOrderIds.Contains(task.WorkOrderId));
        }

        return Ok(tasks);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskResource resource)
    {
        var task = new Task(
            0,
            resource.MechanicId,
            resource.Description,
            resource.Status,
            resource.Priority,
            resource.EstimatedTime,
            resource.LaborPrice,
            resource.LaborCost);

        var result = await taskService.UpdateAsync(id, task);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> PatchTask(int id, [FromBody] PatchTaskResource resource)
    {
        var result = await taskService.PatchStatusAsync(
            id,
            resource.Status ?? string.Empty,
            resource.TechnicalDiagnosis ?? string.Empty,
            resource.CustomerExplanation ?? string.Empty,
            resource.InternalObservation ?? string.Empty,
            resource.EvidenceRegistered ?? string.Empty,
            resource.AdminReviewStatus ?? string.Empty);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await taskService.DeleteAsync(id);
        return NoContent();
    }
}
