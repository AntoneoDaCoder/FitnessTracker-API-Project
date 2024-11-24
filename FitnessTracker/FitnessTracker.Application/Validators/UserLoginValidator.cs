using FluentValidation;
using FitnessTracker.Core.Abstractions;
namespace FitnessTracker.Application.Validators
{
    public class UserLoginDtoValidator : AbstractValidator<ILoginData>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(u => u.Login).NotEmpty().MinimumLength(5).MaximumLength(20);
            RuleFor(u => u.Password).NotEmpty();
        }
    }
}
