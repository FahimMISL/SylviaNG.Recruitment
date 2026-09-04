using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetById
{
    public class FinalSelectionPoolGetByIdHandler : IRequestHandler<FinalSelectionPoolGetByIdQuery, FinalSelectionPoolResponse>
    {
        private readonly IFinalSelectionPoolService _finalSelectionPoolService;

        public FinalSelectionPoolGetByIdHandler(IFinalSelectionPoolService finalSelectionPoolService)
        {
            _finalSelectionPoolService = finalSelectionPoolService;
        }

        public async Task<FinalSelectionPoolResponse> Handle(FinalSelectionPoolGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _finalSelectionPoolService.GetByIdAsync(query.FinalSelectionPoolId);
        }
    }
}
