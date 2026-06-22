using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetById
{
    public class CandidateCertificationGetByIdHandler : IRequestHandler<CandidateCertificationGetByIdQuery, CandidateCertificationResponse>
    {
        private readonly ICandidateCertificationService _service;

        public CandidateCertificationGetByIdHandler(ICandidateCertificationService service)
        {
            _service = service;
        }

        public async Task<CandidateCertificationResponse> Handle(CandidateCertificationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateCertificationId);
        }
    }
}
