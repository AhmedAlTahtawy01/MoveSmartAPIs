using DataAccessLayer.Util; // For ConnectionSettings
using DataAccessLayer.Repositories; // For UserRepo, Sparepart
using BusinessLayer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayer.Services;

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
builder.Services.AddScoped<Sparepart>(); // Kept for Kamal
builder.Services.AddScoped<Vehicleconsumable>(); // Kept for Kamal
//builder.Services.AddScoped<SparePartsPurchaseOrderRepo>(); // Kept for Kamal
//builder.Services.AddScoped<consumablespurchaseorderRepo>(); // Kept for Kamal
builder.Services.AddScoped<ApplicationRepo>(); // Kept for Kamal
builder.Services.AddScoped<SparePartPurchaseOrderService>();
builder.Services.AddScoped<ApplicationService>();


// Register services
builder.Services.AddScoped<UserService>(); // For UserController

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