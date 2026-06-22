using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetById
{
    public class TalentPoolGetByIdHandler : IRequestHandler<TalentPoolGetByIdQuery, TalentPoolResponse>
    {
        private readonly ITalentPoolService _service;

        public TalentPoolGetByIdHandler(ITalentPoolService service)
        {
            _service = service;
        }

        public async Task<TalentPoolResponse> Handle(TalentPoolGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.TalentPoolId);
        }
    }
}
