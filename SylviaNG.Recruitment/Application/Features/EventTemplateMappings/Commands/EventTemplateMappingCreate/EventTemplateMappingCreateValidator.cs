using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingCreate
{
    public class EventTemplateMappingCreateValidator : AbstractValidator<EventTemplateMappingCreateCommand>
    {
        public EventTemplateMappingCreateValidator()
        {
            RuleFor(x => x.Request.RecruitmentEvent).IsInEnum();
            RuleFor(x => x.Request.Channel).IsInEnum();
            RuleFor(x => x.Request.RecipientType).IsInEnum();
            RuleFor(x => x.Request.NotificationTemplateId).GreaterThan(0).WithMessage("A NotificationTemplateId is required.");
        }
    }
}
