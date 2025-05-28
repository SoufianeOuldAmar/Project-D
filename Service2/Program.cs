using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Service2.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<FlightTouchpointDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FlightTouchpointDb")
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
