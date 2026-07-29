using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationSubmit
{
    public class InterviewEvaluationSubmitHandler : IRequestHandler<InterviewEvaluationSubmitCommand, long>
    {
        private readonly IInterviewEvaluationService _interviewEvaluationService;

        public InterviewEvaluationSubmitHandler(IInterviewEvaluationService interviewEvaluationService)
        {
            _interviewEvaluationService = interviewEvaluationService;
        }

        public async Task<long> Handle(InterviewEvaluationSubmitCommand command, CancellationToken cancellationToken)
        {
            return await _interviewEvaluationService.SubmitAsync(command.InterviewId, command.Request);
        }
    }
}
