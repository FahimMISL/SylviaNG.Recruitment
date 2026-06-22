using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetAllPaged
{
    public class EmergencyContactGetAllPagedHandler : IRequestHandler<EmergencyContactGetAllPagedQuery, PagedResult<EmergencyContactResponse>>
    {
        private readonly IEmergencyContactService _emergencyContactService;

        public EmergencyContactGetAllPagedHandler(IEmergencyContactService emergencyContactService)
        {
            _emergencyContactService = emergencyContactService;
        }

        public async Task<PagedResult<EmergencyContactResponse>> Handle(EmergencyContactGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _emergencyContactService.GetPaginatedAsync(query.Request);
        }
    }
}
