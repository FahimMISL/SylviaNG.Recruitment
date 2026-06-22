using MediatR;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.EmergencyContacts.Queries.EmergencyContactGetAllPaged
{
    public class EmergencyContactGetAllPagedQuery : IRequest<PagedResult<EmergencyContactResponse>>
    {
        public PagedRequest Request { get; set; }

        public EmergencyContactGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
