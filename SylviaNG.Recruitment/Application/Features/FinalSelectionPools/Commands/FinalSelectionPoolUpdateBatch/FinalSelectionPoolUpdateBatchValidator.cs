using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolUpdateBatch
{
    public class FinalSelectionPoolUpdateBatchValidator : AbstractValidator<FinalSelectionPoolUpdateBatchCommand>
    {
        public FinalSelectionPoolUpdateBatchValidator()
        {
            RuleFor(x => x.FinalSelectionPoolId)
                .GreaterThan(0).WithMessage("FinalSelectionPoolId is required.");

            RuleFor(x => x.Request.BatchLabel)
                .MaximumLength(200).WithMessage("BatchLabel must not exceed 200 characters.");
        }
    }
}
