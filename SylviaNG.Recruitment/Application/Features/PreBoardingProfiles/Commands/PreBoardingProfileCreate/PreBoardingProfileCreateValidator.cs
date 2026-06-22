using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileCreate
{
    public class PreBoardingProfileCreateValidator : AbstractValidator<PreBoardingProfileCreateCommand>
    {
        public PreBoardingProfileCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
