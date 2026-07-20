using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetAll
{
    public class ExamHallGetAllHandler : IRequestHandler<ExamHallGetAllQuery, List<ExamHallResponse>>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallGetAllHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<List<ExamHallResponse>> Handle(ExamHallGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _examHallService.GetAllAsync();
        }
    }
}
