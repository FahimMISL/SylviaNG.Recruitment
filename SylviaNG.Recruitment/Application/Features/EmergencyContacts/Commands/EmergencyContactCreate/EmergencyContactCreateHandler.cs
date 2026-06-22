using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Commands.EmergencyContactCreate
{
    public class EmergencyContactCreateHandler : IRequestHandler<EmergencyContactCreateCommand, long>
    {
        private readonly IEmergencyContactService _service;

        public EmergencyContactCreateHandler(IEmergencyContactService service)
        {
            _service = service;
        }

        public async Task<long> Handle(EmergencyContactCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
