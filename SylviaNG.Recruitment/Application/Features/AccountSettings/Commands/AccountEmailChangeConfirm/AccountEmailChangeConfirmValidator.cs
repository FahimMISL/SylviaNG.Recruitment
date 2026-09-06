using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Commands.AccountEmailChangeConfirm
{
    public class AccountEmailChangeConfirmValidator : AbstractValidator<AccountEmailChangeConfirmCommand>
    {
        public AccountEmailChangeConfirmValidator()
        {
            RuleFor(x => x.Request.ChallengeId).NotEmpty().WithMessage("ChallengeId is required.");
            RuleFor(x => x.Request.OtpCode).NotEmpty().WithMessage("Code is required.");
        }
    }
}
