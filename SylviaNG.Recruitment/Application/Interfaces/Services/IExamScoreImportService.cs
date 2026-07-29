using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>US-059: downloadable per-exam score-upload template and bulk XLSX/CSV import,
    /// mirroring IExamQuestionImportService's pattern (US-054).</summary>
    public interface IExamScoreImportService
    {
        Task<ExamScoreImportTemplateResponse> GenerateTemplateAsync(long examId);
        Task<ExamScoreBulkUploadResponse> ImportAsync(long examId, IFormFile file);
    }
}
