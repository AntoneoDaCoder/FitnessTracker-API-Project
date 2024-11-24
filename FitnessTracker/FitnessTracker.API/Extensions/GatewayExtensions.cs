using FitnessTracker.Application.Services;
using FitnessTracker.Core.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace FitnessTracker.API.Extensions
{
    public static class GatewayExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestRouter,GatewayRequestRouter>();
        }
        public static void ConfigureHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient
            (
                "auth-service", client =>
                {
                    client.BaseAddress = new Uri("http://auth-service:8081/");
                }
            );
            services.AddHttpClient
                (
                "request-service", client =>
                {
                    client.BaseAddress = new Uri("http://request-service:8082/");
                }
                );
        }
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration conf)
        {
            var jwtSettings = conf.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
    }
}
