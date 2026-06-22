using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeCreate
{
    public class NomineeCreateValidator : AbstractValidator<NomineeCreateCommand>
    {
        public NomineeCreateValidator()
        {
            RuleFor(x => x.Request.PreBoardingProfileId).GreaterThan(0).WithMessage("PreBoardingProfileId is required.");
        }
    }
}
