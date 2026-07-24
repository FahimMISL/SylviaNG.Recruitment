using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Commands.NotificationTemplateCreate
{
    public class NotificationTemplateCreateValidator : AbstractValidator<NotificationTemplateCreateCommand>
    {
        public NotificationTemplateCreateValidator()
        {
            RuleFor(x => x.Request.Channel)
                .IsInEnum().WithMessage("A valid Channel is required.");

            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(100).WithMessage("Code must not exceed 100 characters.");

            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Request.Subject)
                .MaximumLength(300).WithMessage("Subject must not exceed 300 characters.");

            RuleFor(x => x.Request.Subject)
                .NotEmpty().WithMessage("Subject is required for Email templates.")
                .When(x => x.Request.Channel == NotificationChannelEnum.Email);

            RuleFor(x => x.Request.Body)
                .NotEmpty().WithMessage("Body is required.");
        }
    }
}
