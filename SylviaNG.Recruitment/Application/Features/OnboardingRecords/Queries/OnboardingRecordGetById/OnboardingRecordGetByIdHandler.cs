using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetById
{
    public class OnboardingRecordGetByIdHandler : IRequestHandler<OnboardingRecordGetByIdQuery, OnboardingRecordResponse>
    {
        private readonly IOnboardingRecordService _service;

        public OnboardingRecordGetByIdHandler(IOnboardingRecordService service)
        {
            _service = service;
        }

        public async Task<OnboardingRecordResponse> Handle(OnboardingRecordGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.OnboardingRecordId);
        }
    }
}
