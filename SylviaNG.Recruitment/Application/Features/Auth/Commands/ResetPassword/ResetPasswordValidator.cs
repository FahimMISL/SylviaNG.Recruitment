using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Request.ChallengeId)
                .NotEmpty().WithMessage("ChallengeId is required.");

            RuleFor(x => x.Request.OtpCode)
                .NotEmpty().WithMessage("OtpCode is required.");

            RuleFor(x => x.Request.NewPassword)
                .NotEmpty().WithMessage("NewPassword is required.")
                .MinimumLength(8).WithMessage("NewPassword must be at least 8 characters.");
        }
    }
}
