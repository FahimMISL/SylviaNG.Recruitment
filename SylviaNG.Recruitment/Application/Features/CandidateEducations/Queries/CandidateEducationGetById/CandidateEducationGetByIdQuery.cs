using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetById
{
    public class CandidateEducationGetByIdQuery : IRequest<CandidateEducationResponse>
    {
        public long CandidateEducationId { get; set; }

        public CandidateEducationGetByIdQuery(long candidateEducationId)
        {
            CandidateEducationId = candidateEducationId;
        }
    }
}
