using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTracker.Core.Abstractions
{
    public interface IDeleteWorkoutData
    {
        string? UserName { get; }
        List<string>? WorkoutIds { get; }
    }
}
