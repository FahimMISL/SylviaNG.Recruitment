using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataCreate
{
    public class FitmentDataCreateValidator : AbstractValidator<FitmentDataCreateCommand>
    {
        public FitmentDataCreateValidator()
        {
            RuleFor(x => x.Request.CandidateId).GreaterThan(0).WithMessage("CandidateId is required.");
        }
    }
}
