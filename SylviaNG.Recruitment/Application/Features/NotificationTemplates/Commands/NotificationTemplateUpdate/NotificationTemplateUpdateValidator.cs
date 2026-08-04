using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateUpdate
{
    public class NotificationTemplateUpdateValidator : AbstractValidator<NotificationTemplateUpdateCommand>
    {
        public NotificationTemplateUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Subject)
                .MaximumLength(300).WithMessage("Subject must not exceed 300 characters.");

            RuleFor(x => x.Request.Body)
                .NotEmpty().WithMessage("Body is required.");
        }
    }
}
