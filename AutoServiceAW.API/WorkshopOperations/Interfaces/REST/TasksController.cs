using AutoServiceAW.API.InventoryManagement.Domain.Services;
using AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates;
using AutoServiceAW.API.WorkshopOperations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceAW.API.WorkshopOperations.Interfaces.REST;


using Task = AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates.Task;

/// <summary>
/// Data Transfer Object (DTO) container used to instantiate and register an execution task.
/// </summary>
/// <param name="WorkOrderId">The parent tracking work order identification key sequence index link.</param>
/// <param name="MechanicId">The allocated technician operator identity index reference, or <see langword="null"/>.</param>
/// <param name="Description">The structural description of the mechanical instruction to execute.</param>
/// <param name="Priority">The urgency triage rating index (e.g., Low, Medium, High).</param>
/// <param name="EstimatedTime">The total expected duration footprint value measured in minutes.</param>
/// <param name="LaborPrice">The specialized fee cost assigned to the manual technician workforce.</param>
public record CreateTaskResource(int WorkOrderId, int? MechanicId, string Description, string Priority, int EstimatedTime, decimal LaborPrice, List<CreateTaskPartResource> Parts);

public record CreateTaskPartResource(int InventoryItemId, int Quantity );
/// <summary>
/// Data Transfer Object (DTO) representation carrying full structural parameters state updates for an active task row.
/// </summary>
/// <param name="Description">The modified operational instruction definition details.</param>
/// <param name="Status">The altered progress milestone tracking identifier label statement.</param>
/// <param name="Priority">The reassigned execution triage priority status.</param>
/// <param name="EstimatedTime">The recalculated time window footprint scale index framework.</param>
/// <param name="LaborPrice">The adjusted monetary cost fee matching internal manual labors.</param>
/// <param name="MechanicId">The newly assigned operator tracking index configuration point link, or <see langword="null"/>.</param>
public record UpdateTaskResource(string Description, string Status, string Priority, int EstimatedTime, decimal LaborPrice, int? MechanicId);

/// <summary>
/// Data Transfer Object (DTO) envelope containing delta criteria definitions required to patch data logs captured by field technicians.
/// </summary>
/// <param name="Status">The technical flow progression phase identifier statement indicator override.</param>
/// <param name="TechnicalDiagnosis">The mechanic's diagnostic analysis summary description logging details.</param>
/// <param name="CustomerExplanation">The non-technical customer-facing transparency translation narrative.</param>
/// <param name="InternalObservation">The internal supervisor auditing and shop operational review notes.</param>
/// <param name="EvidenceRegistered">The media asset path reference collections mapping captured proofs.</param>
/// <param name="AdminReviewStatus">The management authorization or tracking confirmation sign-off flag status.</param>
public record PatchTaskResource(string? Status, string? TechnicalDiagnosis, string? CustomerExplanation, string? InternalObservation, string? EvidenceRegistered, string? AdminReviewStatus);

/// <summary>
/// Exposes RESTful endpoints for managing granular maintenance tasks, handling data tracking query filters, and technician partial status logs patches.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TasksController(ITaskService taskService, IWorkOrderService workOrderService, IInventoryItemService inventoryItemService) : ControllerBase
{
     #region Methods

    /// <summary>
    /// Creates a new operational task execution row mapping under a targeted parent work order aggregate context.
    /// </summary>
    /// <param name="resource">The incoming schema blueprint container holding capacity, labor pricing, and tracking links data.</param>
    /// <returns>An <see cref="IActionResult"/> with 210 Created containing the tracked result structure state entity wrapper.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskResource resource)
    {
        decimal materialsCost = 0;
        foreach (var part in resource.Parts)
        {
            var inventoryItem =
                await inventoryItemService.GetByIdAsync(part.InventoryItemId);

            if (inventoryItem == null)
                continue;

            materialsCost +=
                inventoryItem.UnitPrice * part.Quantity;
        }

        var task = new Task(resource.WorkOrderId, resource.MechanicId, resource.Description, "PENDING", resource.Priority, resource.EstimatedTime, resource.LaborPrice);
        foreach (var part in resource.Parts)
        {
            var inventoryItem =
                await inventoryItemService.GetByIdAsync(part.InventoryItemId);

            if (inventoryItem == null)
                continue;

            task.AddPart(
                new TaskPart(
                    0,
                    inventoryItem.Id,
                    inventoryItem.Name,
                    part.Quantity,
                    inventoryItem.UnitPrice
                )
            );
            if (inventoryItem.Stock < part.Quantity)
            {
                return BadRequest(
                    $"No hay suficiente stock para {inventoryItem.Name}"
                );
            }
            inventoryItem.ConsumeStock(part.Quantity);
            //inventoryItem.ConsumeStock(inventoryItem.Stock - part.Quantity);

            await inventoryItemService.UpdateAsync(
                inventoryItem.Id,
                inventoryItem
            );
        }
        task.UpdateMaterialsCost(materialsCost);
        var result = await taskService.CreateAsync(task);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Extracts a collection array sequence of tasks optionally filtered by parent work folders, technician allocations, or implicit tenant workshop bounds.
    /// </summary>
    /// <param name="workOrderId">The optional target parent folder structural tracking identity identifier to screen.</param>
    /// <param name="mechanicId">The optional technician user index code locator to fetch task queues.</param>
    /// <returns>An <see cref="IActionResult"/> containing the sequence layout of matching task items, or 401 Unauthorized if claims mismatch.</returns>
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
                .Where(wo => wo.WorkshopId == workshopId)
                .Select(wo => wo.Id)
                .ToList();

            tasks = allTasks.Where(t => workshopOrderIds.Contains(t.WorkOrderId));
        }

        return Ok(tasks);
    }

    /// <summary>
    /// Alters structural parameters, time commitments, labor costs, and personnel assignments over an active task tracker row.
    /// </summary>
    /// <param name="id">The target tracking primary index database row key sequence sequence.</param>
    /// <param name="resource">The modified structure data container tracking blueprint property sets.</param>
    /// <returns>An <see cref="IActionResult"/> with 200 OK containing the edited payload, or 404 Not Found.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskResource resource)
    {
        var task = new Task(0, resource.MechanicId, resource.Description, resource.Status, resource.Priority, resource.EstimatedTime, resource.LaborPrice);
        var result = await taskService.UpdateAsync(id, task);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Patches ongoing technical field diagnostics tracking info, media proofs logs, and specific state progressions atomically.
    /// </summary>
    /// <param name="id">The target tracking row identification key index sequence code.</param>
    /// <param name="resource">The delta envelope carrying incoming text annotations metrics or null pointers parameters.</param>
    /// <returns>An <see cref="IActionResult"/> with 200 OK carrying the patched database record results, or 404 Not Found.</returns>
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchTask(int id, [FromBody] PatchTaskResource resource)
    {
        var result = await taskService.PatchStatusAsync(id, resource.Status ?? "", resource.TechnicalDiagnosis ?? "", resource.CustomerExplanation ?? "", resource.InternalObservation ?? "", resource.EvidenceRegistered ?? "", resource.AdminReviewStatus ?? "");
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Irreversibly drops a concrete operational task row completely out of the database tracking registry.
    /// </summary>
    /// <param name="id">The system primary row tracking identifier sequence index to destroy.</param>
    /// <returns>An <see cref="IActionResult"/> with 204 No Content response layout code status.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await taskService.DeleteAsync(id);
        return NoContent();
    }

    #endregion
}