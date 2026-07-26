using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamQuestionImportService
    {
        ExamQuestionImportTemplateResponse GenerateTemplate();
        Task<ExamQuestionBulkImportResponse> ImportAsync(long questionGroupId, IFormFile file);
    }
}
