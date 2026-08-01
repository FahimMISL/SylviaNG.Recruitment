using MediatR;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingCreate
{
    public class EventTemplateMappingCreateCommand : IRequest<long>
    {
        public EventTemplateMappingCreateRequest Request { get; set; }

        public EventTemplateMappingCreateCommand(EventTemplateMappingCreateRequest request)
        {
            Request = request;
        }
    }
}
