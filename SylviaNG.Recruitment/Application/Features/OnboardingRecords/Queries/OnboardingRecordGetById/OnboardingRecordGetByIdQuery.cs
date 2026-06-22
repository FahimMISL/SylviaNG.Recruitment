using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetById
{
    public class OnboardingRecordGetByIdQuery : IRequest<OnboardingRecordResponse>
    {
        public long OnboardingRecordId { get; set; }

        public OnboardingRecordGetByIdQuery(long onboardingRecordId)
        {
            OnboardingRecordId = onboardingRecordId;
        }
    }
}
