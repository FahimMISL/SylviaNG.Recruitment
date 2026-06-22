using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.NotificationLogs.Commands.BulkNotificationSend
{
    public class BulkNotificationSendValidator : AbstractValidator<BulkNotificationSendCommand>
    {
        public BulkNotificationSendValidator()
        {
            RuleFor(x => x.Request.Recipients).NotEmpty().WithMessage("At least one recipient is required.");
            RuleFor(x => x.Request.Message).NotEmpty().MaximumLength(1000);
        }
    }
}
