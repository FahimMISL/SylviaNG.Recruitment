using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetAllPaged
{
    public class CandidateCertificationGetAllPagedQuery : IRequest<PagedResult<CandidateCertificationResponse>>
    {
        public PagedRequest Request { get; set; }

        public CandidateCertificationGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
