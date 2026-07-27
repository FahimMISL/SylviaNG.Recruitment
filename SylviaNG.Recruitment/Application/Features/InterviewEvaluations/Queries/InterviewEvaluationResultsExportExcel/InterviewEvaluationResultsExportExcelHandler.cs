using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationResultsExportExcel
{
    public class InterviewEvaluationResultsExportExcelHandler : IRequestHandler<InterviewEvaluationResultsExportExcelQuery, InterviewEvaluationResultsFileResponse>
    {
        private readonly IInterviewEvaluationService _interviewEvaluationService;

        public InterviewEvaluationResultsExportExcelHandler(IInterviewEvaluationService interviewEvaluationService)
        {
            _interviewEvaluationService = interviewEvaluationService;
        }

        public async Task<InterviewEvaluationResultsFileResponse> Handle(InterviewEvaluationResultsExportExcelQuery query, CancellationToken cancellationToken)
        {
            return await _interviewEvaluationService.ExportResultsExcelAsync(query.InterviewId);
        }
    }
}
