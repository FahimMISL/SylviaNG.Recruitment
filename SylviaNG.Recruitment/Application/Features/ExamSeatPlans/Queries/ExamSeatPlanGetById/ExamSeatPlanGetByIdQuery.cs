using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetById
{
    public class ExamSeatPlanGetByIdQuery : IRequest<ExamSeatPlanResponse>
    {
        public long ExamSeatPlanId { get; set; }

        public ExamSeatPlanGetByIdQuery(long examSeatPlanId)
        {
            ExamSeatPlanId = examSeatPlanId;
        }
    }
}
