using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Application.DTO
{
    public record UserLoginDto : ILoginData
    {
        public string? Login { get; init; }
        public string? Password { get; init; }
    }
}
