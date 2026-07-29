using MediatR;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;

namespace SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Queries.InterviewEvaluationResultsExportExcel
{
    public class InterviewEvaluationResultsExportExcelQuery : IRequest<InterviewEvaluationResultsFileResponse>
    {
        public long InterviewId { get; set; }

        public InterviewEvaluationResultsExportExcelQuery(long interviewId)
        {
            InterviewId = interviewId;
        }
    }
}
