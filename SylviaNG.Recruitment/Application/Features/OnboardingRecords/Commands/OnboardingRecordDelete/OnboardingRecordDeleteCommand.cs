using MediatR;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordDelete
{
    public class OnboardingRecordDeleteCommand : IRequest<Unit>
    {
        public long OnboardingRecordId { get; set; }

        public OnboardingRecordDeleteCommand(long onboardingRecordId)
        {
            OnboardingRecordId = onboardingRecordId;
        }
    }
}
