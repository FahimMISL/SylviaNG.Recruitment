using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAll
{
    public class ExamHallGetAllHandler : IRequestHandler<ExamHallGetAllQuery, List<ExamHallResponse>>
    {
        private readonly IExamHallService _service;

        public ExamHallGetAllHandler(IExamHallService service)
        {
            _service = service;
        }

        public async Task<List<ExamHallResponse>> Handle(ExamHallGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
