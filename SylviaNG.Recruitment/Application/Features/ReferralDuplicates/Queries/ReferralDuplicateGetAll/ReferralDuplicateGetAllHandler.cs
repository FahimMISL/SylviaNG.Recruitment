using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetAll
{
    public class ReferralDuplicateGetAllHandler : IRequestHandler<ReferralDuplicateGetAllQuery, List<ReferralDuplicateResponse>>
    {
        private readonly IReferralDuplicateService _service;

        public ReferralDuplicateGetAllHandler(IReferralDuplicateService service)
        {
            _service = service;
        }

        public async Task<List<ReferralDuplicateResponse>> Handle(ReferralDuplicateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
