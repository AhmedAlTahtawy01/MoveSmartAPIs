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
using Microsoft.OpenApi.Models;
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

var jwtSection = builder.Configuration.GetSection("JWT");
if (jwtSection == null)
{
    throw new InvalidOperationException("JWT section is not configured in appsettings");
}

// Register JWT settings
builder.Services.Configure<JWT>(jwtSection);

// Register JWT as a singleton
var jwtSettings = jwtSection.Get<JWT>() ?? throw new InvalidOperationException("JWT settings are missing in configuration.");
builder.Services.AddSingleton(jwtSettings);

// Register Token Service
builder.Services.AddScoped<TokenService>();

//// Read JWT settings from configuration or environment variables
//var jwtKey = builder.Configuration["JWT:Key"] ?? Environment.GetEnvironmentVariable("JWT__Key");

//if (string.IsNullOrEmpty(jwtKey))
//{
//    throw new InvalidOperationException("JWT:Key is not configured in appsettings or enviornment variables");
//}

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

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
        };
    });

builder.Services.AddAuthorization(options =>
{
    // A policy that allows only Hospital Managers or higher (e.g. SuperUser)
    options.AddPolicy("RequireHospitalManager", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.HospitalManager.ToString()
        ));

    // A policy that allows only General Manager or higher (e.g. SuperUser, HospitalManager)
    options.AddPolicy("RequireGeneralManager", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.HospitalManager.ToString(),
            EnUserRole.GeneralManager.ToString()
        ));

    // A policy that allows only General Supervisor or higher (e.g. SuperUser, HospitalManager, GeneralManager)
    options.AddPolicy("RequireGeneralSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.HospitalManager.ToString(),
            EnUserRole.GeneralManager.ToString(),
            EnUserRole.GeneralSupervisor.ToString()
        ));

    // A policy that allows only AdministrativeSupervisor or higher (e.g. SuperUser, HospitalManager, GeneralManager, General Supervisor)
    options.AddPolicy("RequireAdministrativeSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.HospitalManager.ToString(),
            EnUserRole.GeneralManager.ToString(),
            EnUserRole.GeneralSupervisor.ToString(),
            EnUserRole.AdministrativeSupervisor.ToString()
        ));

    // A policy that allows only PatrolsSupervisor or higher (e.g. SuperUser, HospitalManager, GeneralManager, General Supervisor)
    options.AddPolicy("RequirePatrolsSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.HospitalManager.ToString(),
            EnUserRole.GeneralManager.ToString(),
            EnUserRole.GeneralSupervisor.ToString(),
            EnUserRole.PatrolsSupervisor.ToString()
        ));

    // A policy that allows only WorkshopSupervisor or higher (e.g. SuperUser, HospitalManager, GeneralManager, General Supervisor)
    options.AddPolicy("RequireWorkshopSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.HospitalManager.ToString(),
            EnUserRole.GeneralManager.ToString(),
            EnUserRole.GeneralSupervisor.ToString(),
            EnUserRole.WorkshopSupervisor.ToString()
        ));

    // Policies for specific roles + SuperUser
    // No need to make one for hospital manager because it's already included in the RequireGeneralManager policy

    options.AddPolicy("GeneralManager", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.GeneralManager.ToString()
        ));

    options.AddPolicy("GeneralSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.GeneralSupervisor.ToString()
        ));

    options.AddPolicy("PatrolsSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.PatrolsSupervisor.ToString()
        ));

    options.AddPolicy("AdministrativeSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.AdministrativeSupervisor.ToString()
        ));

    options.AddPolicy("WorkshopSupervisor", policy =>
        policy.RequireRole(
            EnUserRole.SuperUser.ToString(),
            EnUserRole.WorkshopSupervisor.ToString()
        ));
});


// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Move Smart API",
        Version = "v1",
        Description = "API documentation for Move Smart application"
    });
});

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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Move Smart API V1");
        c.RoutePrefix = string.Empty; // <--- Swagger is now at root
    });
//}

app.UseHttpsRedirection();
app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();