using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleCreate
{
    public class RoleCreateValidator : AbstractValidator<RoleCreateCommand>
    {
        public RoleCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.");
        }
    }
}
