using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetAll
{
    public class EmergencyContactGetAllHandler : IRequestHandler<EmergencyContactGetAllQuery, List<EmergencyContactResponse>>
    {
        private readonly IEmergencyContactService _service;

        public EmergencyContactGetAllHandler(IEmergencyContactService service)
        {
            _service = service;
        }

        public async Task<List<EmergencyContactResponse>> Handle(EmergencyContactGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
