using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingRequestCorrection
{
    public class PreBoardingRequestCorrectionValidator : AbstractValidator<PreBoardingRequestCorrectionCommand>
    {
        public PreBoardingRequestCorrectionValidator()
        {
            RuleFor(x => x.PreBoardingSubmissionId).GreaterThan(0);
            RuleFor(x => x.Request.Comment)
                .NotEmpty().WithMessage("A comment is required when requesting corrections.")
                .MaximumLength(1000);
        }
    }
}
