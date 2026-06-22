using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetById
{
    public class CandidateCertificationGetByIdQuery : IRequest<CandidateCertificationResponse>
    {
        public long CandidateCertificationId { get; set; }

        public CandidateCertificationGetByIdQuery(long candidateCertificationId)
        {
            CandidateCertificationId = candidateCertificationId;
        }
    }
}
