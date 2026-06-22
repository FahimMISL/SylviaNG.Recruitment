using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetAllPaged
{
    public class FitmentDataGetAllPagedHandler : IRequestHandler<FitmentDataGetAllPagedQuery, PagedResult<FitmentDataResponse>>
    {
        private readonly IFitmentDataService _fitmentDataService;

        public FitmentDataGetAllPagedHandler(IFitmentDataService fitmentDataService)
        {
            _fitmentDataService = fitmentDataService;
        }

        public async Task<PagedResult<FitmentDataResponse>> Handle(FitmentDataGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _fitmentDataService.GetPaginatedAsync(query.Request);
        }
    }
}
