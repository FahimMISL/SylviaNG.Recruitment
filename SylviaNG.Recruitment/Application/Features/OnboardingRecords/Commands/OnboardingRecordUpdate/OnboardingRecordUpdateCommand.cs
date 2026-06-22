using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordUpdate
{
    public class OnboardingRecordUpdateCommand : IRequest<Unit>
    {
        public long OnboardingRecordId { get; set; }
        public OnboardingRecordUpdateRequest Request { get; set; }

        public OnboardingRecordUpdateCommand(long onboardingRecordId, OnboardingRecordUpdateRequest request)
        {
            OnboardingRecordId = onboardingRecordId;
            Request = request;
        }
    }
}
