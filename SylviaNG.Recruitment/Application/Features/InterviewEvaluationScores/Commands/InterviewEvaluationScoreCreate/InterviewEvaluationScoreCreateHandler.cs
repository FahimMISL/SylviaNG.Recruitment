using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Commands.InterviewEvaluationScoreCreate
{
    public class InterviewEvaluationScoreCreateHandler : IRequestHandler<InterviewEvaluationScoreCreateCommand, long>
    {
        private readonly IInterviewEvaluationScoreService _service;

        public InterviewEvaluationScoreCreateHandler(IInterviewEvaluationScoreService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InterviewEvaluationScoreCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
