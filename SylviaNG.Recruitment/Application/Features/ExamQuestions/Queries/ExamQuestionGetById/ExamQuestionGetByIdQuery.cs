using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionGetById
{
    public class ExamQuestionGetByIdQuery : IRequest<ExamQuestionResponse>
    {
        public long ExamQuestionId { get; set; }

        public ExamQuestionGetByIdQuery(long examQuestionId)
        {
            ExamQuestionId = examQuestionId;
        }
    }
}
