using ElevatorSystem.Api.DTOs;
using ElevatorSystem.Api.Middleware;
using ElevatorSystem.Core.Contracts;
using ElevatorSystem.Core.Services;
using Microsoft.OpenApi.Models; // Add this for OpenApi types

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((c) =>
{
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Auth-Key: YourKey",
        Name = "Auth-Key",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "Auth-Key",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Configure CORS to allow requests from the frontend server.
builder.Services.AddCors(options =>
  options.AddDefaultPolicy(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()))); // frontend server url


// Register ElevatorService as a singleton with 4 elevators.
builder.Services.AddSingleton<IElevatorService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ElevatorService>>();
    int numElevators = builder.Configuration.GetValue<int>("ElevatorSettings:ElevatorCount");
    return new ElevatorService(numElevators, logger);
});

// Register the background simulation service to run elevator logic in the background.
builder.Services.AddSingleton<ISimulationBackgroundService, SimulationBackgroundService>();
builder.Services.AddHostedService(sp => (SimulationBackgroundService)sp.GetRequiredService<ISimulationBackgroundService>());


var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();
// Register the BasicAuthMiddleware before other middleware
app.UseMiddleware<AuthenticationMiddleware>();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
