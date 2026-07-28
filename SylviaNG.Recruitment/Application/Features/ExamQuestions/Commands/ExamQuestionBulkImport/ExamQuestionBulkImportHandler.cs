using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionBulkImport
{
    public class ExamQuestionBulkImportHandler : IRequestHandler<ExamQuestionBulkImportCommand, ExamQuestionBulkImportResponse>
    {
        private readonly IExamQuestionImportService _examQuestionImportService;

        public ExamQuestionBulkImportHandler(IExamQuestionImportService examQuestionImportService)
        {
            _examQuestionImportService = examQuestionImportService;
        }

        public async Task<ExamQuestionBulkImportResponse> Handle(ExamQuestionBulkImportCommand command, CancellationToken cancellationToken)
        {
            return await _examQuestionImportService.ImportAsync(command.QuestionGroupId, command.File);
        }
    }
}
