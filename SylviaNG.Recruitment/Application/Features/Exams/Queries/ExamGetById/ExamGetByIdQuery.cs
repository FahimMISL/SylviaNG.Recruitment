using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetById
{
    public class ExamGetByIdQuery : IRequest<ExamResponse>
    {
        public long ExamId { get; set; }

        public ExamGetByIdQuery(long examId)
        {
            ExamId = examId;
        }
    }
}
