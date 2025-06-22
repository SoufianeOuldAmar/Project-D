using Gateway.Data;
using Gateway.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); // <- important!

        builder.ConfigureServices(services =>
        {
            // Remove existing UserDbContext registration
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<UserDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Register in-memory DB for testing
            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Build service provider and seed data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<UserDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var user = new Gateway.Entities.User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Role = "User"
            };

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Password123!");

            db.SaveChanges();
        });
    }
}
