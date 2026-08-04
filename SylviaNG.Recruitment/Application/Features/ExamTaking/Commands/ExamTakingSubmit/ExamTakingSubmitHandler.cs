using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Commands.ExamTakingSubmit
{
    public class ExamTakingSubmitHandler : IRequestHandler<ExamTakingSubmitCommand, ExamSubmitResultResponse>
    {
        private readonly IExamTakingService _examTakingService;

        public ExamTakingSubmitHandler(IExamTakingService examTakingService)
        {
            _examTakingService = examTakingService;
        }

        public async Task<ExamSubmitResultResponse> Handle(ExamTakingSubmitCommand command, CancellationToken cancellationToken)
        {
            return await _examTakingService.SubmitExamAsync(command.ExamEnrollmentId, command.Request);
        }
    }
}
