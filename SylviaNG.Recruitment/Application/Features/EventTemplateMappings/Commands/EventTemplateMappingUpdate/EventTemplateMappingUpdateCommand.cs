using MediatR;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingUpdate
{
    public class EventTemplateMappingUpdateCommand : IRequest<Unit>
    {
        public long EventTemplateMappingId { get; set; }
        public EventTemplateMappingUpdateRequest Request { get; set; }

        public EventTemplateMappingUpdateCommand(long eventTemplateMappingId, EventTemplateMappingUpdateRequest request)
        {
            EventTemplateMappingId = eventTemplateMappingId;
            Request = request;
        }
    }
}
