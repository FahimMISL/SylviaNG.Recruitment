using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetById
{
    public class EmergencyContactGetByIdHandler : IRequestHandler<EmergencyContactGetByIdQuery, EmergencyContactResponse>
    {
        private readonly IEmergencyContactService _service;

        public EmergencyContactGetByIdHandler(IEmergencyContactService service)
        {
            _service = service;
        }

        public async Task<EmergencyContactResponse> Handle(EmergencyContactGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.EmergencyContactId);
        }
    }
}
