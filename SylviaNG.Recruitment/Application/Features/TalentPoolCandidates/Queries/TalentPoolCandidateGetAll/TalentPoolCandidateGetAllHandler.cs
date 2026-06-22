using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetAll
{
    public class TalentPoolCandidateGetAllHandler : IRequestHandler<TalentPoolCandidateGetAllQuery, List<TalentPoolCandidateResponse>>
    {
        private readonly ITalentPoolCandidateService _service;

        public TalentPoolCandidateGetAllHandler(ITalentPoolCandidateService service)
        {
            _service = service;
        }

        public async Task<List<TalentPoolCandidateResponse>> Handle(TalentPoolCandidateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
