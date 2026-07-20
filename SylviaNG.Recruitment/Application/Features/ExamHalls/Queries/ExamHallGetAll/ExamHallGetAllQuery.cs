using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAll
{
    public class ExamHallGetAllQuery : IRequest<List<ExamHallResponse>>
    {
    }
}
