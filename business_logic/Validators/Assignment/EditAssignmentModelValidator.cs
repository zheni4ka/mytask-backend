using Core.DTOs;
using FluentValidation;

namespace business_logic.Validators.Assignment
{
    public class EditAssignmentModelValidator : AbstractValidator<EditAssignmentModel>
    {
        public EditAssignmentModelValidator() 
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid ID.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be a positive integer.");
        }
    }
}
