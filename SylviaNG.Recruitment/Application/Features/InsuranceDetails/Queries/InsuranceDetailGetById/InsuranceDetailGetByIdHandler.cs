using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetById
{
    public class InsuranceDetailGetByIdHandler : IRequestHandler<InsuranceDetailGetByIdQuery, InsuranceDetailResponse>
    {
        private readonly IInsuranceDetailService _service;

        public InsuranceDetailGetByIdHandler(IInsuranceDetailService service)
        {
            _service = service;
        }

        public async Task<InsuranceDetailResponse> Handle(InsuranceDetailGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.InsuranceDetailId);
        }
    }
}
