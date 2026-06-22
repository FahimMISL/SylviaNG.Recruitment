using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactUpdate
{
    public class EmergencyContactUpdateHandler : IRequestHandler<EmergencyContactUpdateCommand, Unit>
    {
        private readonly IEmergencyContactService _service;

        public EmergencyContactUpdateHandler(IEmergencyContactService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(EmergencyContactUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.EmergencyContactId, command.Request);
            return Unit.Value;
        }
    }
}
