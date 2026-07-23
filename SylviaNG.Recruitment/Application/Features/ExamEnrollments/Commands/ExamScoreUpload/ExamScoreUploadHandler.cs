using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamScoreUpload
{
    public class ExamScoreUploadHandler : IRequestHandler<ExamScoreUploadCommand>
    {
        private readonly IExamEnrollmentService _examEnrollmentService;

        public ExamScoreUploadHandler(IExamEnrollmentService examEnrollmentService)
        {
            _examEnrollmentService = examEnrollmentService;
        }

        public async Task Handle(ExamScoreUploadCommand command, CancellationToken cancellationToken)
        {
            await _examEnrollmentService.UploadScoreAsync(command.ExamEnrollmentId, command.Score);
        }
    }
}
