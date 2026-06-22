using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetAllPaged
{
    public class OnboardingRecordGetAllPagedHandler : IRequestHandler<OnboardingRecordGetAllPagedQuery, PagedResult<OnboardingRecordResponse>>
    {
        private readonly IOnboardingRecordService _onboardingRecordService;

        public OnboardingRecordGetAllPagedHandler(IOnboardingRecordService onboardingRecordService)
        {
            _onboardingRecordService = onboardingRecordService;
        }

        public async Task<PagedResult<OnboardingRecordResponse>> Handle(OnboardingRecordGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _onboardingRecordService.GetPaginatedAsync(query.Request);
        }
    }
}
