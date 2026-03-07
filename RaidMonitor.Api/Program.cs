using RaidMonitor.Api.Hubs;
using RaidMonitor.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSystemd();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddScoped<RaidService>();
builder.Services.AddHostedService<RaidMonitorService>();

var allowedOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.MapStaticAssets();
app.MapControllers();
app.MapHub<RaidHub>("/hubs/raid");
app.MapFallbackToFile("index.html");
app.Run();