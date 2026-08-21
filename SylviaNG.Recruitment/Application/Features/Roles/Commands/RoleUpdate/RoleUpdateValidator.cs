using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleUpdate
{
    public class RoleUpdateValidator : AbstractValidator<RoleUpdateCommand>
    {
        public RoleUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.");
        }
    }
}
