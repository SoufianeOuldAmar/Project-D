using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlightGatewayTest;
using Gateway.Data;
using Gateway.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

public class CustomWebApplicationFactory<TProgram> 
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private const string TestSecret   = "x8V9NqvE3KYO6EOWlFwY5L7mA4+P1Z3uKZzXx5XfZ9Zu2Q0U74Od3x2eKN6dY1rJ9HCKL1jUCUY6zkG3u64z6g==";
    private const string TestIssuer   = "FlightInfoGateway";
    private const string TestAudience = "RTHA";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 1) Override configuration so the app's AddJwtBearer picks up test values
        builder
          .UseEnvironment("Testing")
          .ConfigureAppConfiguration((ctx, cfg) =>
          {
              var inMem = new Dictionary<string, string?>
              {
                  ["AppSettings:Token"]    = TestSecret,
                  ["AppSettings:Issuer"]   = TestIssuer,
                  ["AppSettings:Audience"] = TestAudience
              };
              cfg.AddInMemoryCollection(inMem);
          });

        // 2) Replace SQL Server with InMemory EF
        builder.ConfigureServices(services =>
        {
            var desc = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<UserDbContext>));
            if (desc != null) services.Remove(desc);

            services.AddDbContext<UserDbContext>(opts =>
                opts.UseInMemoryDatabase("TestDb"));
        });

        // 3) Once the app is built, use ConfigureTestServices to tweak auth and seed
        builder.ConfigureTestServices(services =>
        {
            // Reconfigure JWT Bearer to use test parameters
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = TestIssuer,
                    ValidateAudience = true,
                    ValidAudience = TestAudience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSecret))
                };
            });

            // Seed test users
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var hasher = new PasswordHasher<User>();

            var regular = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Role = "User"
            };
            regular.PasswordHash = hasher.HashPassword(regular, "Password123!");

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Username = "adminuser",
                Role = "Admin"
            };
            admin.PasswordHash = hasher.HashPassword(admin, "AdminPassword123!");

            db.Users.AddRange(regular, admin);
            db.SaveChanges();

            // Replace the real HTTP handler for FlightExports
            services.AddHttpClient("FlightExports")
                    .ConfigurePrimaryHttpMessageHandler(() => new FakeHandler());

            // And for Touchpoints
            services.AddHttpClient("FlightTouchpoints")
                    .ConfigurePrimaryHttpMessageHandler(() => new FakeHandler());
        });
    }
}
