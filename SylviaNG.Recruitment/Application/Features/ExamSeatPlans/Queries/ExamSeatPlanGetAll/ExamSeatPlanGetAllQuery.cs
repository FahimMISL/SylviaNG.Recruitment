using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetAll
{
    public class ExamSeatPlanGetAllQuery : IRequest<List<ExamSeatPlanResponse>>
    {
    }
}
