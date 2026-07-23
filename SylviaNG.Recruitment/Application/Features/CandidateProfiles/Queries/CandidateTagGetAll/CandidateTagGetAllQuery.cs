using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.CandidateTagGetAll
{
    public class CandidateTagGetAllQuery : IRequest<List<CandidateTagResponse>>
    {
        public long CandidateProfileId { get; set; }

        public CandidateTagGetAllQuery(long candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }
}
