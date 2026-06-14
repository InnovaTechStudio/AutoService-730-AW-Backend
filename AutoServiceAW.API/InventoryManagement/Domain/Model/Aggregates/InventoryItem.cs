namespace AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
/// Represents an Inventory Item aggregate root within the Inventory Management domain.
/// </summary>
public class InventoryItem
{
    #region Properties

    /// <summary>
    /// Gets the unique identifier for the inventory item.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the automatically generated Stock Keeping Unit (SKU) code for the item.
    /// </summary>
    public string Sku { get; private set; }

    /// <summary>
    /// Gets the descriptive name of the item.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the category classification of the inventory item (e.g., Lubricants, Brakes).
    /// </summary>
    public string Category { get; private set; }

    /// <summary>
    /// Gets the manufacturer or brand name of the item.
    /// </summary>
    public string Brand { get; private set; }

    /// <summary>
    /// Gets the standard commercial price per unit.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the current quantity available in storage.
    /// </summary>
    public int Stock { get; private set; }

    /// <summary>
    /// Gets the minimum safety threshold level for warehouse inventory alerts.
    /// </summary>
    public int MinStock { get; private set; }
    
    public string Image { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryItem"/> class and generates a unique internal SKU.
    /// </summary>
    /// <param name="name">The name of the inventory piece.</param>
    /// <param name="category">The category tag classification.</param>
    /// <param name="brand">The item manufacturer commercial brand.</param>
    /// <param name="unitPrice">The default base unit price asset value.</param>
    /// <param name="stock">The initial volume available.</param>
    /// <param name="minStock">The minimum required backup storage amount.</param>
    public InventoryItem(string name, string category, string brand, decimal unitPrice, int stock, int minStock, string image)
    {
        Sku = $"SKU-{new Random().Next(1000, 9999)}";
        Name = name;
        Category = category;
        Brand = brand;
        UnitPrice = unitPrice;
        Stock = stock;
        MinStock = minStock;
        Image = image;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryItem"/> class with default values.
    /// Required by ORMs such as Entity Framework Core for data mapping.
    /// </summary>
    protected InventoryItem()
    {
        Sku = string.Empty;
        Name = string.Empty;
        Category = string.Empty;
        Brand = string.Empty;
        Image = string.Empty;
}

    #endregion

    #region Methods

    /// <summary>
    /// Updates the stock details, pricing structure, and metadata tracking configurations.
    /// </summary>
    /// <param name="name">The updated descriptive name.</param>
    /// <param name="category">The updated category target group.</param>
    /// <param name="brand">The updated manufacturer label.</param>
    /// <param name="unitPrice">The updated unit cost scale valuation.</param>
    /// <param name="stock">The current physical stock balance level adjustments.</param>
    /// <param name="minStock">The updated structural low warning security bounds.</param>
    public void Update(string name, string category, string brand, decimal unitPrice, int stock, int minStock, string image)
    {
        Name = name;
        Category = category;
        Brand = brand;
        UnitPrice = unitPrice;
        Stock = stock;
        MinStock = minStock;
        Image = image;
    }

    #endregion
}