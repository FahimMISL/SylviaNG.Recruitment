using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallUpdate
{
    public class ExamHallUpdateHandler : IRequestHandler<ExamHallUpdateCommand, Unit>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallUpdateHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<Unit> Handle(ExamHallUpdateCommand command, CancellationToken cancellationToken)
        {
            await _examHallService.UpdateAsync(command.ExamHallId, command.Request);
            return Unit.Value;
        }
    }
}
