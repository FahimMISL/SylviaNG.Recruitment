using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamAdmitCardDownloadBulkZip
{
    public class ExamAdmitCardDownloadBulkZipHandler : IRequestHandler<ExamAdmitCardDownloadBulkZipQuery, ExamFileResponse>
    {
        private readonly IExamSeatPlanService _examSeatPlanService;

        public ExamAdmitCardDownloadBulkZipHandler(IExamSeatPlanService examSeatPlanService)
        {
            _examSeatPlanService = examSeatPlanService;
        }

        public async Task<ExamFileResponse> Handle(ExamAdmitCardDownloadBulkZipQuery query, CancellationToken cancellationToken)
        {
            var (content, fileName) = await _examSeatPlanService.GenerateAdmitCardZipAsync(query.ExamId);

            return new ExamFileResponse
            {
                Content = content,
                FileName = fileName,
                ContentType = "application/zip"
            };
        }
    }
}
