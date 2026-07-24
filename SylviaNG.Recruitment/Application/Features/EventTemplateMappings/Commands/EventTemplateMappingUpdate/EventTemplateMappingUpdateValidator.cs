using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingUpdate
{
    public class EventTemplateMappingUpdateValidator : AbstractValidator<EventTemplateMappingUpdateCommand>
    {
        public EventTemplateMappingUpdateValidator()
        {
            RuleFor(x => x.Request.NotificationTemplateId).GreaterThan(0).WithMessage("A NotificationTemplateId is required.");
        }
    }
}
