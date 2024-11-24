﻿using FitnessTracker.Core.Abstractions;
using FluentValidation;
namespace FitnessTracker.Application.Validators
{
    public class EditWorkoutDtoValidator : AbstractValidator<IEditWorkoutData>
    {
        public EditWorkoutDtoValidator()
        {
            RuleFor(w => w.Sport).NotEmpty();
            RuleFor(w => w.Date).NotEmpty();
            RuleFor(w => w.Duration).NotEmpty();
            RuleFor(w => w.UserName).NotEmpty();
            RuleFor(w => w.Id).NotEmpty();
            RuleFor(w => w.AveragePulse).NotEmpty().ExclusiveBetween(0, 200);
        }
    }
}