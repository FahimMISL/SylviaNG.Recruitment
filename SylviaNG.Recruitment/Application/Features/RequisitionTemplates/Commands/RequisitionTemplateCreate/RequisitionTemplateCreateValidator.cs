using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.RequisitionTemplates.Commands.RequisitionTemplateCreate
{
    public class RequisitionTemplateCreateValidator : AbstractValidator<RequisitionTemplateCreateCommand>
    {
        public RequisitionTemplateCreateValidator()
        {
            RuleFor(x => x.Request.TemplateName).NotEmpty().WithMessage("TemplateName is required.").MaximumLength(200);
        }
    }
}
