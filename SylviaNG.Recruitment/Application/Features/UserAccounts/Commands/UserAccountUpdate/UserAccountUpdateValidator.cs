using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountUpdate
{
    public class UserAccountUpdateValidator : AbstractValidator<UserAccountUpdateCommand>
    {
        public UserAccountUpdateValidator()
        {
            RuleFor(x => x.Request.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

            RuleFor(x => x.Request.RoleIds)
                .NotEmpty().WithMessage("At least one role must be assigned.");
        }
    }
}
