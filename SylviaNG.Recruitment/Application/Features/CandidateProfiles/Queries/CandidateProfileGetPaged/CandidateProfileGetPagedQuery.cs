using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetPaged
{
    public class CandidateProfileGetPagedQuery : IRequest<PagedResult<CandidateProfileSummaryResponse>>
    {
        public PagedRequest Request { get; set; }
        public List<string>? Tags { get; set; }

        /// <summary>Narrow the list to members of these talent pools (US-039 AC4).</summary>
        public List<long>? TalentPoolIds { get; set; }

        public CandidateProfileGetPagedQuery(PagedRequest request, List<long>? talentPoolIds = null, List<string>? tags = null)
        {
            Request = request;
            TalentPoolIds = talentPoolIds;
            Tags = tags;
        }
    }
}
