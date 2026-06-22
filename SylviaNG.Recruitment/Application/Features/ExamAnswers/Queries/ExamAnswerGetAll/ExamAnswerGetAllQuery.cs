using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetAll
{
    public class ExamAnswerGetAllQuery : IRequest<List<ExamAnswerResponse>>
    {
    }
}
