using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAll
{
    public class FinalSelectionPoolGetAllHandler : IRequestHandler<FinalSelectionPoolGetAllQuery, List<FinalSelectionPoolResponse>>
    {
        private readonly IFinalSelectionPoolService _finalSelectionPoolService;

        public FinalSelectionPoolGetAllHandler(IFinalSelectionPoolService finalSelectionPoolService)
        {
            _finalSelectionPoolService = finalSelectionPoolService;
        }

        public async Task<List<FinalSelectionPoolResponse>> Handle(FinalSelectionPoolGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _finalSelectionPoolService.GetAllAsync();
        }
    }
}
