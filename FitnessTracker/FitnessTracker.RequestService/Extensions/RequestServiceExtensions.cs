using FitnessTracker.Application.Services;
using FitnessTracker.Application.Validators;
using FitnessTracker.Core.Abstractions;
using FluentValidation;
namespace FitnessTracker.RequestService.Extensions
{
    public static class RequestServiceExtensions
    {
        static public void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IClient, RabbitClient>();
            services.AddScoped<IValidator<IAddWorkoutData>, AddWorkoutDtoValidator>();
            services.AddScoped<IValidator<IGetWorkoutData>, GetWorkoutDtoValidator>();
            services.AddScoped<IValidator<IDeleteWorkoutData>, DeleteWorkoutDtoValidator>();
            services.AddScoped<IValidator<IEditWorkoutData>, EditWorkoutDtoValidator>();
            services.AddScoped<IValidator<IGetStats>, StatsDtoValidator>();
        }
    }
}
