using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactDelete
{
    public class EmergencyContactDeleteHandler : IRequestHandler<EmergencyContactDeleteCommand, Unit>
    {
        private readonly IEmergencyContactService _service;

        public EmergencyContactDeleteHandler(IEmergencyContactService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(EmergencyContactDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.EmergencyContactId);
            return Unit.Value;
        }
    }
}
