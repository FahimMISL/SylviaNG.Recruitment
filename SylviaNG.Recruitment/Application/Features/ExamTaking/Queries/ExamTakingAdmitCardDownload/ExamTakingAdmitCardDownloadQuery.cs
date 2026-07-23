using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Queries.ExamTakingAdmitCardDownload
{
    public class ExamTakingAdmitCardDownloadQuery : IRequest<ExamFileResponse>
    {
        public long ExamEnrollmentId { get; set; }

        public ExamTakingAdmitCardDownloadQuery(long examEnrollmentId)
        {
            ExamEnrollmentId = examEnrollmentId;
        }
    }
}
