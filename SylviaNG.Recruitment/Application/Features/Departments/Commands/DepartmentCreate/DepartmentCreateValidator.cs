using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentCreate
{
    public class DepartmentCreateValidator : AbstractValidator<DepartmentCreateCommand>
    {
        public DepartmentCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
