using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamScoreImportTemplateDownload
{
    public class ExamScoreImportTemplateDownloadHandler : IRequestHandler<ExamScoreImportTemplateDownloadQuery, ExamScoreImportTemplateResponse>
    {
        private readonly IExamScoreImportService _examScoreImportService;

        public ExamScoreImportTemplateDownloadHandler(IExamScoreImportService examScoreImportService)
        {
            _examScoreImportService = examScoreImportService;
        }

        public async Task<ExamScoreImportTemplateResponse> Handle(ExamScoreImportTemplateDownloadQuery query, CancellationToken cancellationToken)
        {
            return await _examScoreImportService.GenerateTemplateAsync(query.ExamId);
        }
    }
}
