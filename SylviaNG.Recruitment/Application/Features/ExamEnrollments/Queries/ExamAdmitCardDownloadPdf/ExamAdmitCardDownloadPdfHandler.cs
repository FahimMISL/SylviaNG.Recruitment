using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamAdmitCardDownloadPdf
{
    public class ExamAdmitCardDownloadPdfHandler : IRequestHandler<ExamAdmitCardDownloadPdfQuery, ExamFileResponse>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamAdmitCardDownloadPdfHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task<ExamFileResponse> Handle(ExamAdmitCardDownloadPdfQuery query, CancellationToken cancellationToken)
        {
            var (content, fileName) = await _examSeatPlanService.GenerateAdmitCardPdfAsync(query.ExamEnrollmentId);

            return new ExamFileResponse
            {
                Content = content,
                FileName = fileName,
                ContentType = "application/pdf"
            };
        }
    }
}
