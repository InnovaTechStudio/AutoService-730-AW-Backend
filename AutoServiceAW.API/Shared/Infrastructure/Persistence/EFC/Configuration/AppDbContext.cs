using AutoServiceAW.API.InventoryManagement.Domain.Model.Aggregates;
using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;

using AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Task = AutoServiceAW.API.WorkshopOperations.Domain.Model.Aggregates.Task;

using AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;


namespace AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskPart>  TaskParts { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }

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

        
         builder.Entity<WorkOrder>().ToTable("WorkOrders");
        builder.Entity<WorkOrder>().HasKey(w => w.Id);
        builder.Entity<WorkOrder>().Property(w => w.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WorkOrder>().Property(w => w.WorkshopId).IsRequired().HasMaxLength(50);
        builder.Entity<WorkOrder>().Property(w => w.TrackingCode).IsRequired().HasMaxLength(20);
        builder.Entity<WorkOrder>().Property(w => w.Description).HasMaxLength(500);
        builder.Entity<WorkOrder>().Property(w => w.Status).IsRequired().HasMaxLength(30);
        builder.Entity<WorkOrder>().Property(w => w.Price).HasColumnType("decimal(10,2)");
        builder.Entity<WorkOrder>().Property(w => w.TasksCompleted).HasDefaultValue(false);
        builder.Entity<WorkOrder>().Property(w => w.SparePartsChecked).HasDefaultValue(false);
        builder.Entity<WorkOrder>().Property(w => w.DiagnosisValidated).HasDefaultValue(false);
        builder.Entity<WorkOrder>().Property(w => w.CleaningDone).HasDefaultValue(false);
        builder.Entity<WorkOrder>().Property(w => w.FinalTestDone).HasDefaultValue(false);
        
        // Task mapping
        builder.Entity<Task>().ToTable("Tasks");
        builder.Entity<Task>().HasKey(t => t.Id);
        builder.Entity<Task>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Task>().Property(t => t.Description).IsRequired().HasMaxLength(250);
        builder.Entity<Task>().Property(t => t.Status).IsRequired().HasMaxLength(30);
        builder.Entity<Task>().Property(t => t.Priority).HasMaxLength(20);
        builder.Entity<Task>().Property(t => t.LaborPrice).HasColumnType("decimal(10,2)");
        builder.Entity<Task>() .Property(t => t.MaterialsCost) .HasColumnType("decimal(10,2)");
        builder.Entity<Task>().Ignore(t => t.TotalCost);
        builder.Entity<Task>().Property(t => t.TechnicalDiagnosis).HasMaxLength(1000);
        builder.Entity<Task>().Property(t => t.CustomerExplanation).HasMaxLength(1000);
        builder.Entity<Task>().Property(t => t.InternalObservation).HasMaxLength(500);
        builder.Entity<Task>().Property(t => t.EvidenceRegistered).HasMaxLength(500);
        builder.Entity<Task>().Property(t => t.AdminReviewStatus).HasMaxLength(50);
        
        builder.Entity<Task>().HasOne<WorkOrder>() .WithMany().HasForeignKey(t => t.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        // TaskPart mapping
        builder.Entity<TaskPart>().ToTable("TaskParts");
        builder.Entity<TaskPart>().HasKey(tp => tp.Id);
        builder.Entity<TaskPart>().Property(tp => tp.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TaskPart>().Property(tp => tp.Name).IsRequired().HasMaxLength(150);
        builder.Entity<TaskPart>().Property(tp => tp.UnitPrice).HasColumnType("decimal(10,2)");
        
        builder.Entity<TaskPart>()
            .HasOne(tp => tp.Task)
            .WithMany(t => t.Parts)
            .HasForeignKey(tp => tp.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Workshop>().ToTable("Workshops");
        builder.Entity<Workshop>().HasKey(workshop => workshop.Id);
        builder.Entity<Workshop>().Property(workshop => workshop.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Workshop>().Property(workshop => workshop.Name).IsRequired().HasMaxLength(100);
        builder.Entity<Workshop>().Property(workshop => workshop.TenantId).IsRequired().HasMaxLength(80);
    }
}