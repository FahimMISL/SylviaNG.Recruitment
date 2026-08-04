using MediatR;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingDelete
{
    public class EventTemplateMappingDeleteCommand : IRequest<Unit>
    {
        public long EventTemplateMappingId { get; set; }

        public EventTemplateMappingDeleteCommand(long eventTemplateMappingId)
        {
            EventTemplateMappingId = eventTemplateMappingId;
        }
    }
}
