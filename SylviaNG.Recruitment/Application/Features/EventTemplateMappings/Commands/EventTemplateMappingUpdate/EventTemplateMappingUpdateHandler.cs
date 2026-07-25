using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingUpdate
{
    public class EventTemplateMappingUpdateHandler : IRequestHandler<EventTemplateMappingUpdateCommand, Unit>
    {
        private readonly IEventTemplateMappingService _eventTemplateMappingService;

        public EventTemplateMappingUpdateHandler(IEventTemplateMappingService eventTemplateMappingService)
        {
            _eventTemplateMappingService = eventTemplateMappingService;
        }

        public async Task<Unit> Handle(EventTemplateMappingUpdateCommand command, CancellationToken cancellationToken)
        {
            await _eventTemplateMappingService.UpdateAsync(command.EventTemplateMappingId, command.Request);
            return Unit.Value;
        }
    }
}
