using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Departments.Commands.DepartmentUpdate
{
    public class DepartmentUpdateValidator : AbstractValidator<DepartmentUpdateCommand>
    {
        public DepartmentUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
