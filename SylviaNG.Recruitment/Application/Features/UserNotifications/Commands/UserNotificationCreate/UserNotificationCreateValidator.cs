using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.UserNotifications.Commands.UserNotificationCreate;

public class UserNotificationCreateValidator : AbstractValidator<UserNotificationCreateCommand>
{
    public UserNotificationCreateValidator()
    {
        RuleFor(x => x.Request.KeycloakUserId)
            .NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(300);
        RuleFor(x => x.Request.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000);
    }
}
