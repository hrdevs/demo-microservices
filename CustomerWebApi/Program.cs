using CustomerWebApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

/* Database Context Dependency Injection */
var dbHost = "localhost";
var dbName = "dbo_customer";
var dbPassword = "P@ssword";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True";
builder.Services.AddDbContext<CustomerDbContext>(opt => opt.UseSqlServer(connectionString));
/* ===================================== */
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
