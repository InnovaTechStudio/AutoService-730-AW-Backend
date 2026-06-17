using AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.InventoryManagement.Domain.Repositories;
using AutoServiceAW.API.InventoryManagement.Domain.Services;
using AutoServiceAW.API.Shared.Domain.Repositories;

namespace AutoServiceAW.API.InventoryManagement.Application.Internal;

/// <summary>
/// Provides internal application services for managing inventory item operations.
/// </summary>
public class InventoryItemService(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork) : IInventoryItemService
{
    #region Methods

    /// <summary>
    /// Creates a new inventory item in the system asynchronously.
    /// </summary>
    /// <param name="item">The inventory item entity to be created.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="InventoryItem"/> entity.</returns>
    public async Task<InventoryItem?> CreateAsync(InventoryItem item)
    {
        await inventoryItemRepository.AddAsync(item);
        await unitOfWork.CompleteAsync();
        return item;
    }

    /// <summary>
    /// Retrieves all inventory items registered in the system asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="InventoryItem"/> entities.</returns>
    public async Task<IEnumerable<InventoryItem>> ListAsync()
    {
        return await inventoryItemRepository.ListAsync();
    }
    
    /// <summary>
    /// Retrieves a specific inventory item by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory item.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the found <see cref="InventoryItem"/>, or <see langword="null"/> if not found.</returns>
    public async Task<InventoryItem?> GetByIdAsync(int id) => await inventoryItemRepository.FindByIdAsync(id);

    /// <summary>
    /// Updates an existing inventory item's details asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory item to update.</param>
    /// <param name="updatedItem">The inventory item entity containing the updated information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="InventoryItem"/>, or <see langword="null"/> if the item does not exist.</returns>
    public async Task<InventoryItem?> UpdateAsync(int id, InventoryItem updatedItem)
    {
        var existingItem = await inventoryItemRepository.FindByIdAsync(id);
        if (existingItem == null) return null;

        existingItem.Update(updatedItem.Name, updatedItem.Category, updatedItem.Brand, updatedItem.UnitPrice, updatedItem.Stock, updatedItem.MinStock, updatedItem.Image);        inventoryItemRepository.Update(existingItem);
        await unitOfWork.CompleteAsync();
        return existingItem;
    }

    /// <summary>
    /// Deletes a specific inventory item from the system by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the inventory item to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteAsync(int id)
    {
        var existingItem = await inventoryItemRepository.FindByIdAsync(id);
        if (existingItem != null)
        {
            inventoryItemRepository.Remove(existingItem);
            await unitOfWork.CompleteAsync();
        }
    }
    
    public async Task<bool> ConsumeStockAsync(
        int inventoryItemId,
        int quantity)
    {
        var item =
            await inventoryItemRepository
                .FindByIdAsync(inventoryItemId);

        if (item == null)
            return false;

        item.DecreaseStock(quantity);

        inventoryItemRepository.Update(item);

        await unitOfWork.CompleteAsync();

        return true;
    }
    
    #endregion
}