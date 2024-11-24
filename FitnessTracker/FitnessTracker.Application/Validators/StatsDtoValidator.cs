using FitnessTracker.Core.Abstractions;
using FluentValidation;

namespace FitnessTracker.Application.Validators
{
    public class StatsDtoValidator:AbstractValidator<IGetStats>
    {
        public StatsDtoValidator()
        {
            RuleFor(s=>s.UserName).NotEmpty();
        }
    }
}
