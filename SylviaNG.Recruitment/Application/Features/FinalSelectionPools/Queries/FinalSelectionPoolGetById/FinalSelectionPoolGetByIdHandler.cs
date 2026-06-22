using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetById
{
    public class FinalSelectionPoolGetByIdHandler : IRequestHandler<FinalSelectionPoolGetByIdQuery, FinalSelectionPoolResponse>
    {
        private readonly IFinalSelectionPoolService _service;

        public FinalSelectionPoolGetByIdHandler(IFinalSelectionPoolService service)
        {
            _service = service;
        }

        public async Task<FinalSelectionPoolResponse> Handle(FinalSelectionPoolGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.FinalSelectionPoolId);
        }
    }
}
