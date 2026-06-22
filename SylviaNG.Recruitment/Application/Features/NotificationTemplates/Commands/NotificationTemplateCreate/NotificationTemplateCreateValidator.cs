using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate
{
    public class NotificationTemplateCreateValidator : AbstractValidator<NotificationTemplateCreateCommand>
    {
        public NotificationTemplateCreateValidator()
        {
            RuleFor(x => x.Request.TemplateName)
                .NotEmpty().WithMessage("TemplateName is required.")
                .MaximumLength(500).WithMessage("TemplateName must not exceed 500 characters.");
        }
    }
}
