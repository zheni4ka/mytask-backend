using business_logic.DTOs;
using FluentValidation;

namespace business_logic.Validators.Step
{
    public class EditStepModelValidator : AbstractValidator<EditStepModel>
    {
        public EditStepModelValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        }
    }
}
