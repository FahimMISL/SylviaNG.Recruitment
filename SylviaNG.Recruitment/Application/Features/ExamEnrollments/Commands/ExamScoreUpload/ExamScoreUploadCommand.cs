using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamScoreUpload
{
    public class ExamScoreUploadCommand : IRequest
    {
        public long ExamEnrollmentId { get; set; }
        public decimal Score { get; set; }

        public ExamScoreUploadCommand(long examEnrollmentId, decimal score)
        {
            ExamEnrollmentId = examEnrollmentId;
            Score = score;
        }
    }
}
