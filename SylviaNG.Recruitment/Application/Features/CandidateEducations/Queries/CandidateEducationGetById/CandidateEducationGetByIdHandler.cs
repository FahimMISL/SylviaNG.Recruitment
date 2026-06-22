using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetById
{
    public class CandidateEducationGetByIdHandler : IRequestHandler<CandidateEducationGetByIdQuery, CandidateEducationResponse>
    {
        private readonly ICandidateEducationService _service;

        public CandidateEducationGetByIdHandler(ICandidateEducationService service)
        {
            _service = service;
        }

        public async Task<CandidateEducationResponse> Handle(CandidateEducationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateEducationId);
        }
    }
}
