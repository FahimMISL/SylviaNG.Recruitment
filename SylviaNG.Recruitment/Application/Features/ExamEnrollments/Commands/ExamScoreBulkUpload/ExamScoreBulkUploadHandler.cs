using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamScoreBulkUpload
{
    public class ExamScoreBulkUploadHandler : IRequestHandler<ExamScoreBulkUploadCommand, ExamScoreBulkUploadResponse>
    {
        private readonly IExamScoreImportService _examScoreImportService;

        public ExamScoreBulkUploadHandler(IExamScoreImportService examScoreImportService)
        {
            _examScoreImportService = examScoreImportService;
        }

        public async Task<ExamScoreBulkUploadResponse> Handle(ExamScoreBulkUploadCommand command, CancellationToken cancellationToken)
        {
            return await _examScoreImportService.ImportAsync(command.ExamId, command.File);
        }
    }
}
