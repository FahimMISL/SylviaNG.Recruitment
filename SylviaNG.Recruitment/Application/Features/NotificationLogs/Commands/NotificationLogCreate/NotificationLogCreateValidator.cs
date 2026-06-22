using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.NotificationLogCreate
{
    public class NotificationLogCreateValidator : AbstractValidator<NotificationLogCreateCommand>
    {
        public NotificationLogCreateValidator()
        {
            RuleFor(x => x.Request.NotificationEventId).GreaterThan(0).WithMessage("NotificationEventId is required.");
        }
    }
}
