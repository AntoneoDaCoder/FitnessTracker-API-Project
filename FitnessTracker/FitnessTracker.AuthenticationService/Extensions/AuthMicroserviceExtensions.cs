using FitnessTracker.Application.Services;
using FitnessTracker.Application.Validators;
using FitnessTracker.Core.Abstractions;
using FitnessTracker.Core.Models;
using FitnessTracker.DataAccess.Contexts;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.AuthenticationService.Extensions
{
    public static class AuthMicroserviceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IValidator<IRegisterData>, UserRegisterDtoValidator>();
            services.AddScoped<IValidator<ILoginData>, UserLoginDtoValidator>();
            services.AddScoped<IAuthService, AuthService>();
        }
        public static void ConfigureDbContext(this IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<FitnessTrackerDbContext>
                (
                options => options.UseNpgsql(connectionString)
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
