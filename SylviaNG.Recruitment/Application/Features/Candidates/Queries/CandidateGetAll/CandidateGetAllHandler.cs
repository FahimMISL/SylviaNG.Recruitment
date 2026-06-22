using MediatR;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Candidates.Queries.CandidateGetAll
{
    public class CandidateGetAllHandler : IRequestHandler<CandidateGetAllQuery, List<CandidateResponse>>
    {
        private readonly ICandidateService _service;

        public CandidateGetAllHandler(ICandidateService service)
        {
            _service = service;
        }

        public async Task<List<CandidateResponse>> Handle(CandidateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
