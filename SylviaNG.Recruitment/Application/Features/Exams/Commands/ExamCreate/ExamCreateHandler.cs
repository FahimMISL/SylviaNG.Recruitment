using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Commands.ExamCreate
{
    public class ExamCreateHandler : IRequestHandler<ExamCreateCommand, long>
    {
        private readonly IExamService _examService;

        public ExamCreateHandler(IExamService examService)
        {
            _examService = examService;
        }

        public async Task<long> Handle(ExamCreateCommand command, CancellationToken cancellationToken)
        {
            return await _examService.CreateAsync(command.Request);
        }
    }
}
