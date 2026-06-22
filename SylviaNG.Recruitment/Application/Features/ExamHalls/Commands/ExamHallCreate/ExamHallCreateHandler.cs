using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Commands.ExamHallCreate
{
    public class ExamHallCreateHandler : IRequestHandler<ExamHallCreateCommand, long>
    {
        private readonly IExamHallService _service;

        public ExamHallCreateHandler(IExamHallService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ExamHallCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
