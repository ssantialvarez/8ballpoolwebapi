using _8BallPool.Business.Interfaces;
using _8BallPool.Business.Modules;
using _8BallPool.Data;
using _8BallPool.Data.Interfaces;
using _8BallPool.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

builder.Services.AddDbContext<_8BallPoolContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPlayerModule, PlayerModule>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

// 1. Add Authentication Services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-4yhmgnvzoefgdi26.us.auth0.com/";
    options.Audience = "http://localhost:5234";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. Enable authentication middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
