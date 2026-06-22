using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAll
{
    public class TalentPoolGetAllHandler : IRequestHandler<TalentPoolGetAllQuery, List<TalentPoolResponse>>
    {
        private readonly ITalentPoolService _service;

        public TalentPoolGetAllHandler(ITalentPoolService service)
        {
            _service = service;
        }

        public async Task<List<TalentPoolResponse>> Handle(TalentPoolGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
