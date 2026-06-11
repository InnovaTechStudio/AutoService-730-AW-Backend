// using System.Text;

// Customer Management - NO SE USARÁ POR AHORA
// using AutoServiceAW.API.CustomerManagement.Application.Internal;
// using AutoServiceAW.API.CustomerManagement.Domain.Repositories;
// using AutoServiceAW.API.CustomerManagement.Domain.Services;
// using AutoServiceAW.API.CustomerManagement.Infrastructure.Persistence.EFC.Repositories;

// Fleet Management - NO SE USARÁ POR AHORA
// using AutoServiceAW.API.FleetManagement.Application.Internal;
// using AutoServiceAW.API.FleetManagement.Domain.Repositories;
// using AutoServiceAW.API.FleetManagement.Domain.Services;
// using AutoServiceAW.API.FleetManagement.Infrastructure.Persistence.EFC.Repositories;

// IAM - NO SE USARÁ POR AHORA
// using AutoServiceAW.API.IAM.Application.Internal;
// using AutoServiceAW.API.IAM.Domain.Repositories;
// using AutoServiceAW.API.IAM.Domain.Services;
// using AutoServiceAW.API.IAM.Infrastructure.Persistence.EFC.Repositories;

// Inventory Management - NO SE USARÁ POR AHORA
// using AutoServiceAW.API.InventoryManagement.Application.Internal;
// using AutoServiceAW.API.InventoryManagement.Domain.Repositories;
// using AutoServiceAW.API.InventoryManagement.Domain.Services;
// using AutoServiceAW.API.InventoryManagement.Infrastructure.Persistence.EFC.Repositories;

// Shared - SÍ SE USA porque AppDbContext, UnitOfWork y repositorio base son necesarios para EF Core
using AutoServiceAW.API.Shared.Domain.Repositories;
using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using AutoServiceAW.API.Shared.Infrastructure.Persistence.EFC.Repositories;

// Staff Coordination - SÍ SE USA
using AutoServiceAW.API.StaffCoordination.Application.Internal;
using AutoServiceAW.API.StaffCoordination.Domain.Repositories;
using AutoServiceAW.API.StaffCoordination.Domain.Services;
using AutoServiceAW.API.StaffCoordination.Infrastructure.Persistence.EFC.Repositories;

// Tenant Management - SÍ SE USA
using AutoServiceAW.API.TenantManagement.Application.Internal;
using AutoServiceAW.API.TenantManagement.Domain.Repositories;
using AutoServiceAW.API.TenantManagement.Domain.Services;
using AutoServiceAW.API.TenantManagement.Infrastructure.Persistence.EFC.Repositories;

// Workshop Operations - NO SE USARÁ POR AHORA
// using AutoServiceAW.API.WorkshopOperations.Application.Internal;
// using AutoServiceAW.API.WorkshopOperations.Domain.Repositories;
// using AutoServiceAW.API.WorkshopOperations.Domain.Services;
// using AutoServiceAW.API.WorkshopOperations.Infrastructure.Persistence.EFC.Repositories;

// JWT - NO SE USARÁ POR AHORA
// using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

// JWT - NO SE USARÁ POR AHORA
// using Microsoft.IdentityModel.Tokens;

/// <summary>
/// The main entry point configuration file for the Web Application.
/// Sets up the dependency injection container, database context connections, and HTTP pipelines.
/// For this stage of the project, only StaffCoordination and TenantManagement bounded contexts are enabled.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

#region Controller and API Explorer Configuration

// Add framework service controller endpoints to the DI container.
builder.Services.AddControllers();

// Configure minimal API and controller descriptive route explorer mechanisms.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

// Enforce uniform lower-case formatting policies on all REST endpoint routes and URL structures.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

#endregion

#region Database Connection Configuration

// Extract the relational primary database connection string setting sequence.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString != null)
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

#endregion

#region Dependency Injection Registrations

// Shared Context Boundary Infrastructure Registrations
// Se mantiene porque los repositorios y servicios usan UnitOfWork.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Staff Coordination Bounded Context Registrations
// Este bounded context sí será trabajado.
builder.Services.AddScoped<IMechanicRepository, MechanicRepository>();
builder.Services.AddScoped<IMechanicService, MechanicService>();

// Inventory Management Bounded Context Registrations
// NO SE USARÁ POR AHORA
// builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
// builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();

// Customer Management Bounded Context Registrations
// NO SE USARÁ POR AHORA
// builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// builder.Services.AddScoped<ICustomerService, CustomerService>();

// Fleet Management Bounded Context Registrations
// NO SE USARÁ POR AHORA
// builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
// builder.Services.AddScoped<IVehicleService, VehicleService>();

// Workshop Operations Bounded Context Registrations
// NO SE USARÁ POR AHORA
// builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
// builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
// builder.Services.AddScoped<ITaskRepository, TaskRepository>();
// builder.Services.AddScoped<ITaskService, TaskService>();

// Identity and Access Management (IAM) Bounded Context Registrations
// NO SE USARÁ POR AHORA
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<IAuthService, AuthService>();

// Tenant Management Bounded Context Registrations
// Este bounded context sí será trabajado.
builder.Services.AddScoped<IWorkshopRepository, WorkshopRepository>();
builder.Services.AddScoped<IWorkshopService, WorkshopService>();

#endregion

#region Security and Authentication Infrastructure

// JWT NO SE USARÁ POR AHORA.
// Lo dejamos comentado para no afectar a tus compañeros cuando luego quieran activar IAM.

/*
// Extract JWT token validation configurations properties.
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
*/

// Configure open cross-origin sharing policies to authorize client UI system components integrations.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

#endregion

#region Middleware Execution Pipeline

var app = builder.Build();

#region EnsureCreated Configuration

// EnsureCreated crea la base de datos automáticamente si todavía no existe.
// Lo usaremos para evitar migraciones por ahora.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

#endregion

// Enable Swagger API testing documentation panels.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Inject CORS evaluations and authorization checkpoints to the active pipeline execution flow.
app.UseCors("AllowAll");

// JWT NO SE USARÁ POR AHORA.
// Si luego se activa IAM, se debe descomentar AddAuthentication arriba y también esta línea.
// app.UseAuthentication();

app.UseAuthorization();

// Expose mapping routes matching operational controller boundaries.
app.MapControllers();

// Execute the async background web runner container.
app.Run();

#endregion