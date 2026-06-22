using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetById
{
    public class CandidateGetByIdHandler : IRequestHandler<CandidateGetByIdQuery, CandidateResponse>
    {
        private readonly ICandidateService _service;

        public CandidateGetByIdHandler(ICandidateService service)
        {
            _service = service;
        }

        public async Task<CandidateResponse> Handle(CandidateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateId);
        }
    }
}
