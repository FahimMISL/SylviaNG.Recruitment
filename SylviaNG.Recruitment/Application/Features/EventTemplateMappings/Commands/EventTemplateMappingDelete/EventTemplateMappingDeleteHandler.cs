using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EventTemplateMappings.Commands.EventTemplateMappingDelete
{
    public class EventTemplateMappingDeleteHandler : IRequestHandler<EventTemplateMappingDeleteCommand, Unit>
    {
        private readonly IEventTemplateMappingService _eventTemplateMappingService;

        public EventTemplateMappingDeleteHandler(IEventTemplateMappingService eventTemplateMappingService)
        {
            _eventTemplateMappingService = eventTemplateMappingService;
        }

        public async Task<Unit> Handle(EventTemplateMappingDeleteCommand command, CancellationToken cancellationToken)
        {
            await _eventTemplateMappingService.DeleteAsync(command.EventTemplateMappingId);
            return Unit.Value;
        }
    }
}
