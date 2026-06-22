using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetAll
{
    public class OnboardingRecordGetAllHandler : IRequestHandler<OnboardingRecordGetAllQuery, List<OnboardingRecordResponse>>
    {
        private readonly IOnboardingRecordService _service;

        public OnboardingRecordGetAllHandler(IOnboardingRecordService service)
        {
            _service = service;
        }

        public async Task<List<OnboardingRecordResponse>> Handle(OnboardingRecordGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
