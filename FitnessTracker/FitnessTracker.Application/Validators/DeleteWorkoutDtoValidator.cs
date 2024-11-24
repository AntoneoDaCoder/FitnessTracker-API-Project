

using FitnessTracker.Core.Abstractions;
using FluentValidation;

namespace FitnessTracker.Application.Validators
{
    public class DeleteWorkoutDtoValidator : AbstractValidator<IDeleteWorkoutData>
    {
        public DeleteWorkoutDtoValidator()
        {
            RuleFor(w => w.UserName).NotEmpty();
            RuleFor(w => w.WorkoutIds).NotEmpty();
        }
    }
}
