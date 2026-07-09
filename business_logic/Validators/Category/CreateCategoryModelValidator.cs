using Core.DTOs;
using FluentValidation;

namespace business_logic.Validators.Category
{
    public class CreateCategoryModelValidator : AbstractValidator<CreateCategoryModel>
    {
        public CreateCategoryModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
