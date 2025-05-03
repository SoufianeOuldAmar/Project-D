using Microsoft.EntityFrameworkCore;
using Project_D.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS setup for React (http://localhost:3000)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Allow React app (localhost:3000) to make requests
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add DbContext for database connection
builder.Services.AddDbContext<Vlucht2024ExportDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FlightExportDb")
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Use CORS policy before Authorization
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
