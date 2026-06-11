using AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;
using AutoServiceAW.API.TenantManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Mechanic> Mechanics { get; set; }
    public DbSet<Workshop> Workshops { get; set; }

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

        builder.Entity<Workshop>().ToTable("Workshops");
        builder.Entity<Workshop>().HasKey(workshop => workshop.Id);
        builder.Entity<Workshop>().Property(workshop => workshop.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Workshop>().Property(workshop => workshop.Name).IsRequired().HasMaxLength(100);
        builder.Entity<Workshop>().Property(workshop => workshop.TenantId).IsRequired().HasMaxLength(80);
    }
}