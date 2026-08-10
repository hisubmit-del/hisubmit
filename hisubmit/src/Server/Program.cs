using HiSubmit.Infrastructure.Contexts;
using HiSubmit.Server.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HiSubmit.Server;

public class Program
{
    public static async Task Main(string[] args)
    {    
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            //AddFileFolder();

            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<BlazorHeroContext>();

                if (context.Database.IsSqlServer())
                {
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                throw;
            }
        }

        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStaticWebAssets();
                webBuilder.UseStartup<Startup>();
            });
}
