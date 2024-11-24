using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTracker.Core.Abstractions
{
    public interface IEditWorkoutData
    {
        string? UserName { get; }
        string? Id { get; }
        int AveragePulse { get; }
        DateOnly Date { get; }
        TimeOnly Duration { get; }
        string? Sport { get; }
    }
}
