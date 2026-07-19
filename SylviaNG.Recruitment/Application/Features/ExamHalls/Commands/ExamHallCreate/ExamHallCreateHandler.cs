using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate
{
    public class ExamHallCreateHandler : IRequestHandler<ExamHallCreateCommand, long>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallCreateHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<long> Handle(ExamHallCreateCommand command, CancellationToken cancellationToken)
        {
            return await _examHallService.CreateAsync(command.Request);
        }
    }
}
