using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.RequisitionAttachments.Commands.RequisitionAttachmentCreate
{
    public class RequisitionAttachmentCreateValidator : AbstractValidator<RequisitionAttachmentCreateCommand>
    {
        public RequisitionAttachmentCreateValidator()
        {
            RuleFor(x => x.Request.RequisitionId).GreaterThan(0).WithMessage("RequisitionId is required.");
            RuleFor(x => x.Request.FileName).NotEmpty().WithMessage("FileName is required.").MaximumLength(500);
        }
    }
}
