using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolMarkHasJoined
{
    public class FinalSelectionPoolMarkHasJoinedValidator : AbstractValidator<FinalSelectionPoolMarkHasJoinedCommand>
    {
        public FinalSelectionPoolMarkHasJoinedValidator()
        {
            RuleFor(x => x.FinalSelectionPoolId)
                .GreaterThan(0).WithMessage("FinalSelectionPoolId is required.");
        }
    }
}
