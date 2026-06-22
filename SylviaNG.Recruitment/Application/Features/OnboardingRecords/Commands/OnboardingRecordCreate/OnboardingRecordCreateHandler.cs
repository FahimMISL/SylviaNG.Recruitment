using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordCreate
{
    public class OnboardingRecordCreateHandler : IRequestHandler<OnboardingRecordCreateCommand, long>
    {
        private readonly IOnboardingRecordService _service;

        public OnboardingRecordCreateHandler(IOnboardingRecordService service)
        {
            _service = service;
        }

        public async Task<long> Handle(OnboardingRecordCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
