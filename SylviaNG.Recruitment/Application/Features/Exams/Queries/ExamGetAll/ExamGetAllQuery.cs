using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAll
{
    public class ExamGetAllQuery : IRequest<List<ExamResponse>>
    {
    }
}
