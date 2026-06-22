using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordUpdate
{
    public class OnboardingRecordUpdateHandler : IRequestHandler<OnboardingRecordUpdateCommand, Unit>
    {
        private readonly IOnboardingRecordService _service;

        public OnboardingRecordUpdateHandler(IOnboardingRecordService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(OnboardingRecordUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.OnboardingRecordId, command.Request);
            return Unit.Value;
        }
    }
}
