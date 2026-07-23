using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamResultsExportExcel
{
    public class ExamResultsExportExcelHandler : IRequestHandler<ExamResultsExportExcelQuery, ExamFileResponse>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamResultsExportExcelHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task<ExamFileResponse> Handle(ExamResultsExportExcelQuery query, CancellationToken cancellationToken)
        {
            var (content, fileName) = await _examSeatPlanService.GenerateResultsExcelAsync(query.ExamId);

            return new ExamFileResponse
            {
                Content = content,
                FileName = fileName,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
    }
}
