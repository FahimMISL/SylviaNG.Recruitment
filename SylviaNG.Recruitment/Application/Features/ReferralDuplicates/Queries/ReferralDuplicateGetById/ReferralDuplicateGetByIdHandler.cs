using MediatR;
using SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ReferralDuplicates.Queries.ReferralDuplicateGetById
{
    public class ReferralDuplicateGetByIdHandler : IRequestHandler<ReferralDuplicateGetByIdQuery, ReferralDuplicateResponse>
    {
        private readonly IReferralDuplicateService _service;

        public ReferralDuplicateGetByIdHandler(IReferralDuplicateService service)
        {
            _service = service;
        }

        public async Task<ReferralDuplicateResponse> Handle(ReferralDuplicateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ReferralDuplicateId);
        }
    }
}
