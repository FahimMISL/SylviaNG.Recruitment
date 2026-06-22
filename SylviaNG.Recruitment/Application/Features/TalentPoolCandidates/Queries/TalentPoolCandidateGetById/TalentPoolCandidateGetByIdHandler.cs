using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Queries.TalentPoolCandidateGetById
{
    public class TalentPoolCandidateGetByIdHandler : IRequestHandler<TalentPoolCandidateGetByIdQuery, TalentPoolCandidateResponse>
    {
        private readonly ITalentPoolCandidateService _service;

        public TalentPoolCandidateGetByIdHandler(ITalentPoolCandidateService service)
        {
            _service = service;
        }

        public async Task<TalentPoolCandidateResponse> Handle(TalentPoolCandidateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.TalentPoolCandidateId);
        }
    }
}
