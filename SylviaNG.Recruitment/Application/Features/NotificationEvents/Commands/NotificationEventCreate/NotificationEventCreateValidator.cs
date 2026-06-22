using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.NotificationEvents.Commands.NotificationEventCreate
{
    public class NotificationEventCreateValidator : AbstractValidator<NotificationEventCreateCommand>
    {
        public NotificationEventCreateValidator()
        {
            RuleFor(x => x.Request.EventName).NotEmpty().WithMessage("EventName is required.").MaximumLength(200);
        }
    }
}
