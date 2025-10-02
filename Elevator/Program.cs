using ElevatorApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS to allow requests from the frontend server.
builder.Services.AddCors(options =>
  options.AddDefaultPolicy(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()))); // frontend server url

// Register ElevatorService as a singleton with 4 elevators.
builder.Services.AddSingleton<ElevatorService>(sp =>
    new ElevatorService(
        builder.Configuration.GetValue<int>("ElevatorSettings:ElevatorCount"),
        sp.GetRequiredService<ILogger<ElevatorService>>()
    ));

// Register the background simulation service to run elevator logic in the background.
builder.Services.AddHostedService<SimulationBackgroundService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors(); 

if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.Run();
