using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Commands.RequisitionApprovalCreate
{
    public class RequisitionApprovalCreateValidator : AbstractValidator<RequisitionApprovalCreateCommand>
    {
        public RequisitionApprovalCreateValidator()
        {
            RuleFor(x => x.Request.RequisitionId).GreaterThan(0).WithMessage("RequisitionId is required.");
        }
    }
}
