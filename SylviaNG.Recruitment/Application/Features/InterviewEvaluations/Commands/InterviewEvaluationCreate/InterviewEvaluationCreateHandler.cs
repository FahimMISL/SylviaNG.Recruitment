using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Commands.InterviewEvaluationCreate
{
    public class InterviewEvaluationCreateHandler : IRequestHandler<InterviewEvaluationCreateCommand, long>
    {
        private readonly IInterviewEvaluationService _service;

        public InterviewEvaluationCreateHandler(IInterviewEvaluationService service)
        {
            _service = service;
        }

        public async Task<long> Handle(InterviewEvaluationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
