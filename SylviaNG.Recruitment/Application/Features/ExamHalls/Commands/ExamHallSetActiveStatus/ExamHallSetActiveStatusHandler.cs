using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallSetActiveStatus
{
    public class ExamHallSetActiveStatusHandler : IRequestHandler<ExamHallSetActiveStatusCommand, Unit>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallSetActiveStatusHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<Unit> Handle(ExamHallSetActiveStatusCommand command, CancellationToken cancellationToken)
        {
            await _examHallService.SetActiveStatusAsync(command.ExamHallId, command.IsActive);
            return Unit.Value;
        }
    }
}
