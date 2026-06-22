using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigCreate
{
    public class RequisitionStageConfigCreateValidator : AbstractValidator<RequisitionStageConfigCreateCommand>
    {
        public RequisitionStageConfigCreateValidator()
        {
            RuleFor(x => x.Request.RequisitionId).GreaterThan(0).WithMessage("RequisitionId is required.");
            RuleFor(x => x.Request.StageName).NotEmpty().WithMessage("StageName is required.").MaximumLength(200);
        }
    }
}
