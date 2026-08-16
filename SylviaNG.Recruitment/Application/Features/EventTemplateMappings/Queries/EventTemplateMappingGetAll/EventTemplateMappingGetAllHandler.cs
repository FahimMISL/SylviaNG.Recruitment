using MediatR;
using SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Queries.EventTemplateMappingGetAll
{
    public class EventTemplateMappingGetAllHandler : IRequestHandler<EventTemplateMappingGetAllQuery, List<EventTemplateMappingResponse>>
    {
        private readonly IEventTemplateMappingService _eventTemplateMappingService;

        public EventTemplateMappingGetAllHandler(IEventTemplateMappingService eventTemplateMappingService)
        {
            _eventTemplateMappingService = eventTemplateMappingService;
        }

        public async Task<List<EventTemplateMappingResponse>> Handle(EventTemplateMappingGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _eventTemplateMappingService.GetAllAsync();
        }
    }
}
