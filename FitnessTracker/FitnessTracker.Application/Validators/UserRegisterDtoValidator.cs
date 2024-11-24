using FluentValidation;
using FitnessTracker.Core.Models;
using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Application.Validators
{
    public class UserRegisterDtoValidator : AbstractValidator<IRegisterData>
    {
        public UserRegisterDtoValidator()
        {
            RuleFor(u => u.Login).NotEmpty().MinimumLength(5).MaximumLength(20);
            RuleFor(u => u.Password).NotEmpty();
            RuleFor(u => u.Age).NotEmpty().InclusiveBetween((uint)18, (uint)99);
            RuleFor(u => u.Weight).NotEmpty().GreaterThanOrEqualTo(40.0d);
            RuleFor(u => u.Height).NotEmpty().GreaterThanOrEqualTo(0u);
            RuleFor(u => u.Gender).NotEmpty().Must(g => g == "male" || g == "female");
        }
    }
}
