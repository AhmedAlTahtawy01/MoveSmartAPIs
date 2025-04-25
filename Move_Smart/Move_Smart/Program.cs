using DataAccessLayer.Util; // For ConnectionSettings
using DataAccessLayer.Repositories; // For UserRepo, Sparepart
using BusinessLayer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayer.Services;
using DataAccessLayer;
using BusinessLayer;
using Move_Smart.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IncludeFields = true; // Moved from unused variable
    });
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    );

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// ConnectionSettings for raw SQL
builder.Services.AddSingleton<ConnectionSettings>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<ConnectionSettings>>();
    return new ConnectionSettings(config, logger);
});

// Register repositories
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<ApplicationRepo>();
builder.Services.AddScoped<JobOrderRepo>();
builder.Services.AddScoped<MaintenanceRepo>();
builder.Services.AddScoped<MissionRepo>();
builder.Services.AddScoped<MissionsJobOrderRepo>();
builder.Services.AddScoped<MissionsVehicleRepo>();
builder.Services.AddScoped<Sparepart>(); // Kept for Kamal
builder.Services.AddScoped<Vehicleconsumable>(); // Kept for Kamal
//builder.Services.AddScoped<SparePartsPurchaseOrderRepo>(); // Kept for Kamal
//builder.Services.AddScoped<consumablespurchaseorderRepo>(); // Kept for Kamal
builder.Services.AddScoped<ApplicationRepo>(); // Kept for Kamal
builder.Services.AddScoped<VehicleRepo>();
builder.Services.AddScoped<BusRepo>();
builder.Services.AddScoped<DriverRepo>();
builder.Services.AddScoped<EmployeeRepo>();
builder.Services.AddScoped<PatrolRepo>();
builder.Services.AddScoped<VacationRepo>();
builder.Services.AddScoped<SparePartPurchaseOrderService>();
builder.Services.AddScoped<ConsumablespurchaseorderService>();
builder.Services.AddScoped<CosumableWithdawApplicationService>();



// Register services
builder.Services.AddScoped<UserService>(); // For UserController
builder.Services.AddScoped<ApplicationService>(); // For JobOrderService
builder.Services.AddScoped<JobOrderService>(); // For JobOrderController
builder.Services.AddScoped<MaintenanceService>(); // For MaintenanceController
builder.Services.AddScoped<MissionService>(); // For MissionController
builder.Services.AddScoped<MissionsJobOrderService>(); // For MissionsJobOrderController
builder.Services.AddScoped<MissionsVehicleService>(); // For MissionsVehicleController
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<BusService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<PatrolService>();
builder.Services.AddScoped<VacationService>();

// EF Core for Kamal's work
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<appDBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.36-mysql")));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IncludeFields = true;
    });


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization(); // No auth enforced, but kept for future use
app.MapControllers();

app.Run();