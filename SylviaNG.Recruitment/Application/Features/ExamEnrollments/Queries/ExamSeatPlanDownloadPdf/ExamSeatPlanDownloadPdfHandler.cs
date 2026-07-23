using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamSeatPlanDownloadPdf
{
    public class ExamSeatPlanDownloadPdfHandler : IRequestHandler<ExamSeatPlanDownloadPdfQuery, ExamFileResponse>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamSeatPlanDownloadPdfHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task<ExamFileResponse> Handle(ExamSeatPlanDownloadPdfQuery query, CancellationToken cancellationToken)
        {
            var (content, fileName) = await _examSeatPlanService.GeneratePdfAsync(query.ExamId);

            return new ExamFileResponse
            {
                Content = content,
                FileName = fileName,
                ContentType = "application/pdf"
            };
        }
    }
}
