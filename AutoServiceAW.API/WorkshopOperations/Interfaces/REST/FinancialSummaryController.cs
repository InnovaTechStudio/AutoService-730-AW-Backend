using AutoServiceAW.API.WorkshopOperations.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates.Task;

namespace AutoServiceAW.API.WorkshopOperations.Interfaces.REST;

public record FinancialPeriodPointResource(
    string Date,
    string Label,
    decimal Revenue,
    decimal Cost,
    decimal Profit);

public record OrderProfitabilityResource(
    int WorkOrderId,
    string TrackingCode,
    decimal Revenue,
    decimal Cost,
    decimal Profit,
    decimal MarginPercentage);

public record FinancialSummaryResource(
    decimal ProjectedRevenue,
    decimal RealizedRevenue,
    decimal PendingRevenue,
    decimal OperatingCost,
    decimal GrossProfit,
    decimal MarginPercentage,
    int ProfitableOrders,
    int LossOrders,
    decimal AverageTicket,
    IReadOnlyCollection<FinancialPeriodPointResource> PeriodSeries,
    IReadOnlyCollection<OrderProfitabilityResource> OrderProfitability);

[ApiController]
[Route("api/v1/financial-summary")]
[Authorize]
public class FinancialSummaryController(
    IWorkOrderService workOrderService,
    ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSummary([FromQuery] int days = 7)
    {
        days = Math.Clamp(days, 7, 90);

        var workshopId = User.Claims.FirstOrDefault(claim => claim.Type == "WorkshopId")?.Value;
        if (string.IsNullOrWhiteSpace(workshopId))
            return Unauthorized();

        var orders = (await workOrderService.ListByTenantIdAsync(workshopId))
            .Where(order => !string.Equals(order.Status, "CANCELLED", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var orderIds = orders.Select(order => order.Id).ToHashSet();
        var tasks = (await taskService.ListAsync())
            .Where(task => orderIds.Contains(task.WorkOrderId))
            .ToList();

        var profitability = orders
            .Select(order => BuildOrderProfitability(
                order.Id,
                order.TrackingCode,
                tasks.Where(task => task.WorkOrderId == order.Id)))
            .ToList();

        var projectedRevenue = profitability.Sum(item => item.Revenue);
        var operatingCost = profitability.Sum(item => item.Cost);
        var grossProfit = projectedRevenue - operatingCost;
        var margin = projectedRevenue == 0
            ? 0
            : Math.Round((grossProfit / projectedRevenue) * 100, 2);

        var deliveredOrderIds = orders
            .Where(order => string.Equals(order.Status, "DELIVERED", StringComparison.OrdinalIgnoreCase))
            .Select(order => order.Id)
            .ToHashSet();

        var realizedRevenue = profitability
            .Where(item => deliveredOrderIds.Contains(item.WorkOrderId))
            .Sum(item => item.Revenue);

        var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));
        var periodSeries = Enumerable.Range(0, days)
            .Select(offset => startDate.AddDays(offset))
            .Select(date =>
            {
                var dateOrderIds = orders
                    .Where(order => DateOnly.TryParse(order.StartDate, out var parsedDate) && parsedDate == DateOnly.FromDateTime(date))
                    .Select(order => order.Id)
                    .ToHashSet();

                var dateProfitability = profitability
                    .Where(item => dateOrderIds.Contains(item.WorkOrderId))
                    .ToList();

                var revenue = dateProfitability.Sum(item => item.Revenue);
                var cost = dateProfitability.Sum(item => item.Cost);

                return new FinancialPeriodPointResource(
                    date.ToString("yyyy-MM-dd"),
                    date.ToString("ddd").ToUpperInvariant(),
                    revenue,
                    cost,
                    revenue - cost);
            })
            .ToList();

        return Ok(new FinancialSummaryResource(
            projectedRevenue,
            realizedRevenue,
            projectedRevenue - realizedRevenue,
            operatingCost,
            grossProfit,
            margin,
            profitability.Count(item => item.Profit >= 0),
            profitability.Count(item => item.Profit < 0),
            profitability.Count == 0 ? 0 : Math.Round(projectedRevenue / profitability.Count, 2),
            periodSeries,
            profitability.OrderByDescending(item => item.Profit).ToList()));
    }

    private static OrderProfitabilityResource BuildOrderProfitability(
        int workOrderId,
        string trackingCode,
        IEnumerable<Task> tasks)
    {
        var taskList = tasks.ToList();
        var revenue = taskList.Sum(task => task.TotalCost);
        var cost = taskList.Sum(task => task.TotalInternalCost);
        var profit = revenue - cost;
        var margin = revenue == 0 ? 0 : Math.Round((profit / revenue) * 100, 2);

        return new OrderProfitabilityResource(
            workOrderId,
            trackingCode,
            revenue,
            cost,
            profit,
            margin);
    }
}
