using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Request.ChallengeId).NotEmpty().WithMessage("ChallengeId is required.");
            RuleFor(x => x.Request.Code).NotEmpty().Length(6).WithMessage("Code must be a 6-digit value.");
        }
    }
}
