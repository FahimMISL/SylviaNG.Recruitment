using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpValidator : AbstractValidator<ResendOtpCommand>
    {
        public ResendOtpValidator()
        {
            RuleFor(x => x.Request.ChallengeId).NotEmpty().WithMessage("ChallengeId is required.");
        }
    }
}
