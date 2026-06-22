using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetAll
{
    public class InsuranceDetailGetAllHandler : IRequestHandler<InsuranceDetailGetAllQuery, List<InsuranceDetailResponse>>
    {
        private readonly IInsuranceDetailService _service;

        public InsuranceDetailGetAllHandler(IInsuranceDetailService service)
        {
            _service = service;
        }

        public async Task<List<InsuranceDetailResponse>> Handle(InsuranceDetailGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
