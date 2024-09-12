using Microsoft.EntityFrameworkCore;
using Serilog;
using Bowl.Data;
using Bowl.Services;
using Bowl.Common;
using Bowl.Services.Daemon;

var builder = WebApplication.CreateBuilder(args);

// Database config
var dbSettings = builder.Configuration.GetSection("Datebase");
var host = dbSettings["host"];
var user = dbSettings["user"];
var password = dbSettings["password"];
var database = dbSettings["database"];
var connectionString = $"server={host};port=3306;database={database};user={user};password={password};";

// Handle front-end CORS
var frontHost = builder.Configuration.GetValue<string>("FrontHost");

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndCorsPolicy", policy =>
    {
        policy.WithOrigins(frontHost)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 6, 51))));
ServicesInitializer.Initialize(builder);
builder.Services.AddControllers();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Utils.Initialize(app.Services, app.Logger);
Daemon.Start();

app.UseCors("FrontEndCorsPolicy");
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();