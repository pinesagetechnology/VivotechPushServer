using System.Text.Json;
using VivoTechPushServer.Models;
using VivoTechPushServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register custom services
builder.Services.AddScoped<DataStorageService>();

// Configure CORS for Vivotek devices
builder.Services.AddCors(options =>
{
    options.AddPolicy("VivotekPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure JSON serialization
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("VivotekPolicy");

app.UseAuthorization();

app.MapControllers();

// Add a simple root endpoint
app.MapGet("/", () => "VivoTech Push Server is running!");

app.Run();
