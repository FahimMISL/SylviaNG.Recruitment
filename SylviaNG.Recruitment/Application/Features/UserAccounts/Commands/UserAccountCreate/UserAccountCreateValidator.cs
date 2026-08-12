using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Commands.UserAccountCreate
{
    public class UserAccountCreateValidator : AbstractValidator<UserAccountCreateCommand>
    {
        public UserAccountCreateValidator()
        {
            RuleFor(x => x.Request.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Request.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

            RuleFor(x => x.Request.RoleIds)
                .NotEmpty().WithMessage("At least one role must be assigned.");
        }
    }
}
