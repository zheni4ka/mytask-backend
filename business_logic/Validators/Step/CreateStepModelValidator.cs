using business_logic.DTOs;
using FluentValidation;

namespace business_logic.Validators.Step
{
    public class CreateStepModelValidator : AbstractValidator<CreateStepModel>
    {
        public CreateStepModelValidator() 
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.AssignmentId)
                .GreaterThan(0).WithMessage("Assignment ID must be a positive integer.");
        }
    }
}
