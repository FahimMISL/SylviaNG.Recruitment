using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamSeatPlanDownloadExcel
{
    public class ExamSeatPlanDownloadExcelHandler : IRequestHandler<ExamSeatPlanDownloadExcelQuery, ExamFileResponse>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamSeatPlanDownloadExcelHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task<ExamFileResponse> Handle(ExamSeatPlanDownloadExcelQuery query, CancellationToken cancellationToken)
        {
            var (content, fileName) = await _examSeatPlanService.GenerateExcelAsync(query.ExamId);

            return new ExamFileResponse
            {
                Content = content,
                FileName = fileName,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
    }
}
