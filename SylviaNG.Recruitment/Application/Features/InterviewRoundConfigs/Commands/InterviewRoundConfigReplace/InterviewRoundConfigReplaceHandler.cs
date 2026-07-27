using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewRoundConfigs.Commands.InterviewRoundConfigReplace
{
    public class InterviewRoundConfigReplaceHandler : IRequestHandler<InterviewRoundConfigReplaceCommand>
    {
        private readonly IInterviewRoundConfigService _interviewRoundConfigService;

        public InterviewRoundConfigReplaceHandler(IInterviewRoundConfigService interviewRoundConfigService)
        {
            _interviewRoundConfigService = interviewRoundConfigService;
        }

        public async Task Handle(InterviewRoundConfigReplaceCommand command, CancellationToken cancellationToken)
        {
            await _interviewRoundConfigService.ReplaceForJobPostingAsync(command.JobPostingId, command.Request);
        }
    }
}
