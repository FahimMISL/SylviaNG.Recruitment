using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Commands.OnboardingRecordDelete
{
    public class OnboardingRecordDeleteHandler : IRequestHandler<OnboardingRecordDeleteCommand, Unit>
    {
        private readonly IOnboardingRecordService _service;

        public OnboardingRecordDeleteHandler(IOnboardingRecordService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(OnboardingRecordDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.OnboardingRecordId);
            return Unit.Value;
        }
    }
}
