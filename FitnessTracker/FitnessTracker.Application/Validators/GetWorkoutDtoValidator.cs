using FitnessTracker.Core.Abstractions;
using FluentValidation;
namespace FitnessTracker.Application.Validators
{
    public class GetWorkoutDtoValidator : AbstractValidator<IGetWorkoutData>
    {
        public GetWorkoutDtoValidator()
        {
            RuleFor(w=>w.UserName).NotEmpty();
        }
    }
}
