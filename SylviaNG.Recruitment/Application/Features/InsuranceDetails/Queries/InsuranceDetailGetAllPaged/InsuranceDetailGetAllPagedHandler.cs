using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetAllPaged
{
    public class InsuranceDetailGetAllPagedHandler : IRequestHandler<InsuranceDetailGetAllPagedQuery, PagedResult<InsuranceDetailResponse>>
    {
        private readonly IInsuranceDetailService _insuranceDetailService;

        public InsuranceDetailGetAllPagedHandler(IInsuranceDetailService insuranceDetailService)
        {
            _insuranceDetailService = insuranceDetailService;
        }

        public async Task<PagedResult<InsuranceDetailResponse>> Handle(InsuranceDetailGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _insuranceDetailService.GetPaginatedAsync(query.Request);
        }
    }
}
