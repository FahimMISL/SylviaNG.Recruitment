using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingCreate
{
    public class EventTemplateMappingCreateHandler : IRequestHandler<EventTemplateMappingCreateCommand, long>
    {
        private readonly IEventTemplateMappingService _eventTemplateMappingService;

        public EventTemplateMappingCreateHandler(IEventTemplateMappingService eventTemplateMappingService)
        {
            _eventTemplateMappingService = eventTemplateMappingService;
        }

        public async Task<long> Handle(EventTemplateMappingCreateCommand command, CancellationToken cancellationToken)
        {
            return await _eventTemplateMappingService.CreateAsync(command.Request);
        }
    }
}
