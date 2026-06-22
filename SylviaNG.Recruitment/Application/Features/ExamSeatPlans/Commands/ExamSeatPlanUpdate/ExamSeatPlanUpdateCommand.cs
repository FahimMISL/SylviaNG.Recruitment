using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanUpdate
{
    public class ExamSeatPlanUpdateCommand : IRequest<Unit>
    {
        public long ExamSeatPlanId { get; set; }
        public ExamSeatPlanUpdateRequest Request { get; set; }

        public ExamSeatPlanUpdateCommand(long examSeatPlanId, ExamSeatPlanUpdateRequest request)
        {
            ExamSeatPlanId = examSeatPlanId;
            Request = request;
        }
    }
}
