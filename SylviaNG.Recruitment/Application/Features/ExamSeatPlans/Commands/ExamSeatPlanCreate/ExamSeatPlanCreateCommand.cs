using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Commands.ExamSeatPlanCreate
{
    public class ExamSeatPlanCreateCommand : IRequest<long>
    {
        public ExamSeatPlanCreateRequest Request { get; set; }

        public ExamSeatPlanCreateCommand(ExamSeatPlanCreateRequest request)
        {
            Request = request;
        }
    }
}
