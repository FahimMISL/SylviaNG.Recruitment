using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationUpdate
{
    public class InterviewEvaluationUpdateHandler : IRequestHandler<InterviewEvaluationUpdateCommand>
    {
        private readonly IInterviewEvaluationService _interviewEvaluationService;

        public InterviewEvaluationUpdateHandler(IInterviewEvaluationService interviewEvaluationService)
        {
            _interviewEvaluationService = interviewEvaluationService;
        }

        public async Task Handle(InterviewEvaluationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _interviewEvaluationService.UpdateAsync(command.InterviewEvaluationId, command.Request);
        }
    }
}
