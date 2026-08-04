using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamEnrollments.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Queries.ExamEnrollmentGetByExam
{
    public class ExamEnrollmentGetByExamQuery : IRequest<List<ExamEnrollmentResponse>>
    {
        public long ExamId { get; set; }

        public ExamEnrollmentGetByExamQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
