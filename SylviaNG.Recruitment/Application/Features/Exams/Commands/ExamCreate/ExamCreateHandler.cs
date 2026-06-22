using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate
{
    public class ExamCreateHandler : IRequestHandler<ExamCreateCommand, long>
    {
        private readonly IExamService _service;

        public ExamCreateHandler(IExamService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ExamCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
