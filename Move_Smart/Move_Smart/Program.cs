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
using DataAccessLayer.SharedFunctions;
using BusinessLogicLayer.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BusinessLogicLayer.Helpers;
using Microsoft.Extensions.Options;
using System.Configuration;

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

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddSingleton<JWT>(sp => sp.GetRequiredService<IOptions<JWT>>().Value);


// Register JWT authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT:Key is not configured"))),
        };
    });


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

// Register Shared
builder.Services.AddScoped<SharedFunctions>();

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
builder.Services.AddScoped<MaintenanceApplicationRepo>();
builder.Services.AddScoped<MissionsNotesRepo>();
builder.Services.AddScoped<PatrolsSubscriptionRepo>();
builder.Services.AddScoped<SparePartPurchaseOrderService>();
builder.Services.AddScoped<ConsumablespurchaseorderService>();
builder.Services.AddScoped<CosumableWithdawApplicationService>();
builder.Services.AddScoped<SparePartWithdrawApplicationService>();

// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
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
builder.Services.AddScoped<ConsumablesReplacementRepo>();
builder.Services.AddScoped<SparePartsReplacement>();
builder.Services.AddScoped<VacationService>();

builder.Services.AddSignalR();

builder.Services.AddScoped<MaintenanceApplicationService>();
builder.Services.AddScoped<MissionsNotesService>();
builder.Services.AddScoped<PatrolsSubscriptionService>();



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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();