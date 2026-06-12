using AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;
using AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Mechanic>().ToTable("Mechanics");
        builder.Entity<Mechanic>().HasKey(mechanic => mechanic.Id);
        builder.Entity<Mechanic>().Property(mechanic => mechanic.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Mechanic>().Property(mechanic => mechanic.FullName).IsRequired().HasMaxLength(80);
        builder.Entity<Mechanic>().Property(mechanic => mechanic.Specialty).IsRequired().HasMaxLength(80);
        builder.Entity<Mechanic>().Property(mechanic => mechanic.MaxCapacity).IsRequired();
        builder.Entity<Mechanic>().Property(mechanic => mechanic.Email).IsRequired().HasMaxLength(120);
        builder.Entity<Mechanic>().Property(mechanic => mechanic.Password).IsRequired().HasMaxLength(120);
        
        // InventoryItem mapping
        builder.Entity<InventoryItem>().ToTable("InventoryItems");
        builder.Entity<InventoryItem>().HasKey(i => i.Id);
        builder.Entity<InventoryItem>().Property(i => i.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<InventoryItem>().Property(i => i.Sku).IsRequired().HasMaxLength(20);
        builder.Entity<InventoryItem>().Property(i => i.Name).IsRequired().HasMaxLength(150);
        builder.Entity<InventoryItem>().Property(i => i.Category).IsRequired().HasMaxLength(50);
        builder.Entity<InventoryItem>().Property(i => i.Brand).HasMaxLength(50);
        builder.Entity<InventoryItem>().Property(i => i.UnitPrice).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<InventoryItem>().Property(i => i.Stock).IsRequired().HasDefaultValue(0);
        builder.Entity<InventoryItem>().Property(i => i.MinStock).IsRequired().HasDefaultValue(5);

        builder.Entity<Workshop>().ToTable("Workshops");
        builder.Entity<Workshop>().HasKey(workshop => workshop.Id);
        builder.Entity<Workshop>().Property(workshop => workshop.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Workshop>().Property(workshop => workshop.Name).IsRequired().HasMaxLength(100);
        builder.Entity<Workshop>().Property(workshop => workshop.TenantId).IsRequired().HasMaxLength(80);
    }
}