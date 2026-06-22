using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactCreate
{
    public class EmergencyContactCreateValidator : AbstractValidator<EmergencyContactCreateCommand>
    {
        public EmergencyContactCreateValidator()
        {
            RuleFor(x => x.Request.PreBoardingProfileId).GreaterThan(0).WithMessage("PreBoardingProfileId is required.");
        }
    }
}
