namespace FitnessTracker.Core.Abstractions
{
    public interface IRegisterData
    {
        public string? Login { get; }
        public string? Password { get;  }
        public uint Age { get;  }
        public double Weight { get; }
        public uint Height { get;  }
        public string? Gender { get; }
    }
}