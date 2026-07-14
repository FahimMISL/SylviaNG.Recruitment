using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateProfileGetById
{
    public class CandidateProfileGetByIdQuery : IRequest<CandidateProfileDetailResponse>
    {
        public long CandidateProfileId { get; set; }

        public CandidateProfileGetByIdQuery(long candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }
}
