using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamScoreImportTemplateDownload
{
    public class ExamScoreImportTemplateDownloadQuery : IRequest<ExamScoreImportTemplateResponse>
    {
        public long ExamId { get; set; }

        public ExamScoreImportTemplateDownloadQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
