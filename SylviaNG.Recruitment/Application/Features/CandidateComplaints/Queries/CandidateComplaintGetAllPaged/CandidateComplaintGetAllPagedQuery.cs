using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetAllPaged
{
    public class CandidateComplaintGetAllPagedQuery : IRequest<PagedResult<CandidateComplaintResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateComplaintGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
