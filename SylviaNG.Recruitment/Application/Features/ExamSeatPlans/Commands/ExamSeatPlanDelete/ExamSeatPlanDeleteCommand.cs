using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanDelete
{
    public class ExamSeatPlanDeleteCommand : IRequest<Unit>
    {
        public long ExamSeatPlanId { get; set; }

        public ExamSeatPlanDeleteCommand(long examSeatPlanId)
        {
            ExamSeatPlanId = examSeatPlanId;
        }
    }
}
