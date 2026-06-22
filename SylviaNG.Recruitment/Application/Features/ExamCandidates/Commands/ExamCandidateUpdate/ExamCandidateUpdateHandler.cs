using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateUpdate
{
    public class ExamCandidateUpdateHandler : IRequestHandler<ExamCandidateUpdateCommand, Unit>
    {
        private readonly IExamCandidateService _service;

        public ExamCandidateUpdateHandler(IExamCandidateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamCandidateUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ExamCandidateId, command.Request);
            return Unit.Value;
        }
    }
}
