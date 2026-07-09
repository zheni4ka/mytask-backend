using Core.DTOs;
using FluentValidation;

namespace business_logic.Validators.Step
{
    public class EditStepModelValidator : AbstractValidator<EditStepModel>
    {
        public EditStepModelValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid ID.");

            RuleFor(x => x.AssignmentId).GreaterThan(0).WithMessage("Invalid Assignment ID");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        }
    }
}
