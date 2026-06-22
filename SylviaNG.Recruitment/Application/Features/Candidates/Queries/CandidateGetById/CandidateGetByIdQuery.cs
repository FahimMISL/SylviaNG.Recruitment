using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetById
{
    public class CandidateGetByIdQuery : IRequest<CandidateResponse>
    {
        public long CandidateId { get; set; }

        public CandidateGetByIdQuery(long candidateId)
        {
            CandidateId = candidateId;
        }
    }
}
