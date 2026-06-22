using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Queries.ExamSeatPlanGetById
{
    public class ExamSeatPlanGetByIdHandler : IRequestHandler<ExamSeatPlanGetByIdQuery, ExamSeatPlanResponse>
    {
        private readonly IExamSeatPlanService _service;

        public ExamSeatPlanGetByIdHandler(IExamSeatPlanService service)
        {
            _service = service;
        }

        public async Task<ExamSeatPlanResponse> Handle(ExamSeatPlanGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ExamSeatPlanId);
        }
    }
}
