using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordCreate
{
    public class OnboardingRecordCreateCommand : IRequest<long>
    {
        public OnboardingRecordCreateRequest Request { get; set; }

        public OnboardingRecordCreateCommand(OnboardingRecordCreateRequest request)
        {
            Request = request;
        }
    }
}
