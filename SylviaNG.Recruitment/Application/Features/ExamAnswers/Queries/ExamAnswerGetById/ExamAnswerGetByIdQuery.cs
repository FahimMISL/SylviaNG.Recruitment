using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetById
{
    public class ExamAnswerGetByIdQuery : IRequest<ExamAnswerResponse>
    {
        public long ExamAnswerId { get; set; }

        public ExamAnswerGetByIdQuery(long examAnswerId)
        {
            ExamAnswerId = examAnswerId;
        }
    }
}
