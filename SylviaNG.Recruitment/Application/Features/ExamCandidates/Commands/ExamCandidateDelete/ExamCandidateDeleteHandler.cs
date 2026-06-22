using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateDelete
{
    public class ExamCandidateDeleteHandler : IRequestHandler<ExamCandidateDeleteCommand, Unit>
    {
        private readonly IExamCandidateService _service;

        public ExamCandidateDeleteHandler(IExamCandidateService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ExamCandidateDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ExamCandidateId);
            return Unit.Value;
        }
    }
}
