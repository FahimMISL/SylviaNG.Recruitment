using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountPasswordChange
{
    public class AccountPasswordChangeValidator : AbstractValidator<AccountPasswordChangeCommand>
    {
        public AccountPasswordChangeValidator()
        {
            RuleFor(x => x.Request.CurrentPassword)
                .NotEmpty().WithMessage("CurrentPassword is required.");

            RuleFor(x => x.Request.NewPassword)
                .NotEmpty().WithMessage("NewPassword is required.")
                .MinimumLength(8).WithMessage("NewPassword must be at least 8 characters.")
                .NotEqual(x => x.Request.CurrentPassword).WithMessage("NewPassword must be different from CurrentPassword.");
        }
    }
}
