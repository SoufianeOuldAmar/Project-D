using System.Text;
using Gateway.Data;
using Gateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
        options.HttpsPort = 7150;
    });
}

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var alreadyRegistered = builder.Services.Any(s => s.ServiceType == typeof(DbContextOptions<UserDbContext>));
// Only register SQL Server when not in a test environment
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));
}



// builder.Services.AddDbContext<UserDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"])),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyFrontend", builder =>
        builder.WithOrigins("http://localhost:4200")  // Allow your frontend URL
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient("FlightExports", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FlightExports:BaseUrl"]!);
});

builder.Services.AddHttpClient("FlightTouchpoints", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FlightTouchpoints:BaseUrl"]!);
});
  

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowMyFrontend");

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


public partial class Program { }
// This partial class is used to allow the Program class to be extended in tests or other parts of the application.