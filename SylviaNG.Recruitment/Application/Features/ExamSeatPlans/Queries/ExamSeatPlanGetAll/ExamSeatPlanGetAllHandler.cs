using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetAll
{
    public class ExamSeatPlanGetAllHandler : IRequestHandler<ExamSeatPlanGetAllQuery, List<ExamSeatPlanResponse>>
    {
        private readonly IExamSeatPlanService _service;

        public ExamSeatPlanGetAllHandler(IExamSeatPlanService service)
        {
            _service = service;
        }

        public async Task<List<ExamSeatPlanResponse>> Handle(ExamSeatPlanGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
