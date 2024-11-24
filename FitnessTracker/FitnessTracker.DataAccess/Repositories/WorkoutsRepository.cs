using FitnessTracker.DataAccess.Contexts;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
namespace FitnessTracker.DataAccess.Repositories
{
    public class WorkoutsRepository : IWorkoutsRepository
    {
        private readonly FitnessTrackerDbContext _context;
        private readonly UserManager<User> _userManager;
        enum PulseZones { Undefined, Resting, Light, Moderate, Intensive, SuperIntensive };
        public WorkoutsRepository(UserManager<User> manager, FitnessTrackerDbContext context)
        {
            _context = context;
            _userManager = manager;
        }
        private static PulseZones GetPulseZone(int averagePulse)
        {
            if (averagePulse < 110)
                return PulseZones.Resting;
            if (averagePulse >= 110 && averagePulse < 130)
                return PulseZones.Light;
            if (averagePulse >= 130 && averagePulse < 150)
                return PulseZones.Moderate;
            if (averagePulse >= 150 && averagePulse < 170)
                return PulseZones.Intensive;
            if (averagePulse >= 170 && averagePulse <= 200)
                return PulseZones.SuperIntensive;
            return PulseZones.Undefined;
        }
        private static double GetAMRCoeff(int averagePulse)
        {
            PulseZones zone = GetPulseZone(averagePulse);
            switch (zone)
            {
                case PulseZones.Resting:
                    return 1.2d;
                case PulseZones.Light:
                    return 1.375;
                case PulseZones.Moderate:
                    return 1.55;
                case PulseZones.Intensive:
                    return 1.725;
                case PulseZones.SuperIntensive:
                    return 1.9;
                default:
                    return 0;
            }
        }
        private static double GetFraction(TimeOnly duration)
        {
            var totalSecInDay = 24 * 60 * 60;
            var timeSecs = duration.Hour * 3600 + duration.Minute * 60 + duration.Second;
            return (double)timeSecs / totalSecInDay;
        }
        private static double GetUserCalories(User user, int averagePulse, TimeOnly duration)
        {
            double amrCoeff = GetAMRCoeff(averagePulse);
            double bmr;
            if (user.Gender == "male")
                bmr = 88.362 + (13.397 * user.Weight) + (4.799 * user.Height) - (5.677 * user.Age);
            else
                bmr = 447.593 + (9.247 * user.Weight) + (3.098 * user.Height) - (4.33 * user.Age);

            double amr = amrCoeff * bmr;
            return amr * GetFraction(duration);
        }
        public async Task<string> GetUserWorkoutsAsync(IGetWorkoutData validWorkoutDto)
        {
            var u = await _userManager.Users.Include(u => u.Workouts).AsNoTracking().FirstOrDefaultAsync(u => u.UserName == validWorkoutDto.UserName);
            if (u != null)
            {
                if (validWorkoutDto.WorkoutId != string.Empty)
                {
                    var workout = u.Workouts.FirstOrDefault(w => w.Id == validWorkoutDto.WorkoutId);
                    if (workout != null)
                        return JsonSerializer.Serialize(workout, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
                    else
                        throw new InvalidOperationException("Workout not found.");
                }
                return JsonSerializer.Serialize(u.Workouts, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            throw new InvalidOperationException("User not found.");
        }
        public async Task<string> CreateUserWorkoutAsync(IAddWorkoutData validWorkoutDto)
        {
            var u = await _userManager.Users.Include(u => u.Workouts).FirstOrDefaultAsync(u => u.UserName == validWorkoutDto.UserName);
            if (u != null)
            {
                var spentCalories = GetUserCalories(u, validWorkoutDto.AveragePulse, validWorkoutDto.Duration);
                Workout workout = new Workout()
                {
                    Id = Guid.NewGuid().ToString(),
                    Date = validWorkoutDto.Date,
                    Duration = validWorkoutDto.Duration,
                    Sport = validWorkoutDto.Sport,
                    TotalCalories = spentCalories,
                };
                u.Workouts.Add(workout);
                await _context.SaveChangesAsync();
                return JsonSerializer.Serialize(workout, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            throw new InvalidOperationException("User not found.");
        }
        public async Task<string> DeleteUserWorkoutAsync(IDeleteWorkoutData validWorkoutDto)
        {
            var u = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == validWorkoutDto.UserName);
            if (u != null)
            {
                var result = new Dictionary<string, string>();
                var existingWorkouts = await _context.Workouts.Where(w => w.UserId == u.Id && validWorkoutDto.WorkoutIds.Contains(w.Id)).ToListAsync();
                _context.RemoveRange(existingWorkouts);
                foreach (var existingWorkout in existingWorkouts)
                    result.Add(existingWorkout.Id, "Successfully deleted");

                var missingWorkouts = validWorkoutDto.WorkoutIds.Except(existingWorkouts.Select(w => w.Id));
                foreach (var missingWorkout in missingWorkouts)
                    result.Add(missingWorkout, "Not found");
                await _context.SaveChangesAsync();
                return JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            throw new InvalidOperationException("User not found.");
        }
        public async Task<string> UpdateUserWorkoutAsync(IEditWorkoutData validWorkoutDto)
        {
            var u = await _userManager.Users.Include(u => u.Workouts).FirstOrDefaultAsync(u => u.UserName == validWorkoutDto.UserName);
            if (u != null)
            {
                var spentCalories = GetUserCalories(u, validWorkoutDto.AveragePulse, validWorkoutDto.Duration);
                var workout = u.Workouts.FirstOrDefault(w => w.Id == validWorkoutDto.Id);
                if (workout != null)
                {
                    workout.TotalCalories = spentCalories;
                    workout.Duration = validWorkoutDto.Duration;
                    workout.Sport = validWorkoutDto.Sport;
                    workout.Date = validWorkoutDto.Date;

                    await _context.SaveChangesAsync();
                    return JsonSerializer.Serialize(workout, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                }
                throw new InvalidOperationException("Workout not found.");
            }
            throw new InvalidOperationException("User not found.");
        }
        public async Task<string> GetUserStatsAsync(IGetStats validStatsDto)
        {
            var u = await _userManager.Users.Include(u => u.Workouts).AsNoTracking().FirstOrDefaultAsync(u => u.UserName == validStatsDto.UserName);
            if (u != null)
            {
                var workouts = _context.Workouts.Where(w => w.UserId == u.Id && w.Date >= validStatsDto.From && w.Date <= validStatsDto.To).ToList();
                if (workouts.Any())
                {
                    var stats = workouts.GroupBy(w => 1).Select(g => new
                    {
                        AvgDuration = TimeSpan.FromTicks((long)g.Average(w => w.Duration.ToTimeSpan().Ticks)),
                        MaxDuration = g.Max(w => w.Duration),
                        MinDuration = g.Min(w => w.Duration),
                        AvgCalories = g.Average(w => w.TotalCalories),
                        MinCalories = g.Min(w => w.TotalCalories),
                        MaxCalories = g.Max(w => w.TotalCalories),
                    }).FirstOrDefault();
                    WorkoutStats wStats = new()
                    {
                        AverageCalories = stats.AvgCalories,
                        MinCalories = stats.MinCalories,
                        MaxCalories = stats.MaxCalories,
                        AverageDuration = TimeOnly.FromTimeSpan(stats.AvgDuration),
                        MaxDuration = stats.MaxDuration,
                        MinDuration = stats.MinDuration,
                        Workouts = workouts
                    };
                    return JsonSerializer.Serialize(wStats, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                }
                throw new InvalidOperationException("There are no workouts in the specified time period.");
            }
            throw new InvalidOperationException("User not found.");
        }
    }
}
