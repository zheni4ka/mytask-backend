using business_logic.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic.Validators.Category
{
    public class EditCategoryModelValidator : AbstractValidator<EditCategoryModel>
    {
        public EditCategoryModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
