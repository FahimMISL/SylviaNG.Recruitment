using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Commands.ExamTakingStart
{
    public class ExamTakingStartHandler : IRequestHandler<ExamTakingStartCommand, ExamPaperResponse>
    {
        private readonly IExamTakingService _examTakingService;

        public ExamTakingStartHandler(IExamTakingService examTakingService)
        {
            _examTakingService = examTakingService;
        }

        public async Task<ExamPaperResponse> Handle(ExamTakingStartCommand command, CancellationToken cancellationToken)
        {
            return await _examTakingService.StartExamAsync(command.ExamEnrollmentId);
        }
    }
}
