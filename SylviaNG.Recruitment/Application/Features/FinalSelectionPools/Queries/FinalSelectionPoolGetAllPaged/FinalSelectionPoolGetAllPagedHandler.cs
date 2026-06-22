using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAllPaged
{
    public class FinalSelectionPoolGetAllPagedHandler : IRequestHandler<FinalSelectionPoolGetAllPagedQuery, PagedResult<FinalSelectionPoolResponse>>
    {
        private readonly IFinalSelectionPoolService _finalSelectionPoolService;

        public FinalSelectionPoolGetAllPagedHandler(IFinalSelectionPoolService finalSelectionPoolService)
        {
            _finalSelectionPoolService = finalSelectionPoolService;
        }

        public async Task<PagedResult<FinalSelectionPoolResponse>> Handle(FinalSelectionPoolGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _finalSelectionPoolService.GetPaginatedAsync(query.Request);
        }
    }
}
