using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Application.DTO
{
    public record UserRegisterDto : IRegisterData
    {
        public string? Login { get; init; }
        public string? Password { get; init; }
        public uint Age { get; init; }
        public double Weight { get; init; }
        public uint Height { get; init; }
        public string? Gender { get; init; }
    }
}
