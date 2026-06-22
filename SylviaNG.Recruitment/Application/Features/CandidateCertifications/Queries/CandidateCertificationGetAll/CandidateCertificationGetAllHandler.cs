using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateCertifications.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateCertifications.Queries.CandidateCertificationGetAll
{
    public class CandidateCertificationGetAllHandler : IRequestHandler<CandidateCertificationGetAllQuery, List<CandidateCertificationResponse>>
    {
        private readonly ICandidateCertificationService _service;

        public CandidateCertificationGetAllHandler(ICandidateCertificationService service)
        {
            _service = service;
        }

        public async Task<List<CandidateCertificationResponse>> Handle(CandidateCertificationGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
