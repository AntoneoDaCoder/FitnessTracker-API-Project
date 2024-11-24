using FitnessTracker.Core.Abstractions;
using FitnessTracker.Application.Services;
using FitnessTracker.DataAccess.Repositories;
using FitnessTracker.Core.Models;
using FitnessTracker.DataAccess.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace FitnessTracker.StatisticsService.Extensions
{
    public static class StatisticsExtensions
    {
        static public void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IWorker, RabbitWorker>();
            services.AddScoped<IWorkoutsRepository, WorkoutsRepository>();
            services.AddSingleton<IWorkoutService, StatService>();
        }
        public static void ConfigureDbContext(this IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<FitnessTrackerDbContext>
                (
                options => options.UseNpgsql(connectionString).UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                );
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<FitnessTrackerDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
