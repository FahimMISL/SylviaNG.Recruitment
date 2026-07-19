using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionImportTemplateDownload
{
    public class ExamQuestionImportTemplateDownloadHandler : IRequestHandler<ExamQuestionImportTemplateDownloadQuery, ExamQuestionImportTemplateResponse>
    {
        private readonly IExamQuestionImportService _examQuestionImportService;

        public ExamQuestionImportTemplateDownloadHandler(IExamQuestionImportService examQuestionImportService)
        {
            _examQuestionImportService = examQuestionImportService;
        }

        public Task<ExamQuestionImportTemplateResponse> Handle(ExamQuestionImportTemplateDownloadQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(_examQuestionImportService.GenerateTemplate());
        }
    }
}
