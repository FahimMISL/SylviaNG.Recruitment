using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateCreate
{
    public class ExamCandidateCreateHandler : IRequestHandler<ExamCandidateCreateCommand, long>
    {
        private readonly IExamCandidateService _service;

        public ExamCandidateCreateHandler(IExamCandidateService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ExamCandidateCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
