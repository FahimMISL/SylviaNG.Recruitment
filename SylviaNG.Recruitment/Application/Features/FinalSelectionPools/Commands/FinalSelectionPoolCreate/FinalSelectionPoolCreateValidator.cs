using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolCreate
{
    public class FinalSelectionPoolCreateValidator : AbstractValidator<FinalSelectionPoolCreateCommand>
    {
        public FinalSelectionPoolCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId)
                .GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
