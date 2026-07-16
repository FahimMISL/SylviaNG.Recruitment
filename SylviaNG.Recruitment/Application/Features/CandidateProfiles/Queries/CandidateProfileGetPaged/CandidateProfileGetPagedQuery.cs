using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetPaged
{
    public class CandidateProfileGetPagedQuery : IRequest<PagedResult<CandidateProfileSummaryResponse>>
    {
        public PagedRequest Request { get; set; }
        public List<string>? Tags { get; set; }

        public CandidateProfileGetPagedQuery(PagedRequest request, List<string>? tags = null)
        {
            Request = request;
            Tags = tags;
        }
    }
}
