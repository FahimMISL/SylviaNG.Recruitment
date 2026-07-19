using MediatR;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionBulkImport
{
    public class ExamQuestionBulkImportCommand : IRequest<ExamQuestionBulkImportResponse>
    {
        public long QuestionGroupId { get; set; }
        public IFormFile File { get; set; }

        public ExamQuestionBulkImportCommand(long questionGroupId, IFormFile file)
        {
            QuestionGroupId = questionGroupId;
            File = file;
        }
    }
}
