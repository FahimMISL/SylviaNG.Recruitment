using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetById
{
    public class ExamHallGetByIdHandler : IRequestHandler<ExamHallGetByIdQuery, ExamHallResponse>
    {
        private readonly IExamHallService _examHallService;

        public ExamHallGetByIdHandler(IExamHallService examHallService)
        {
            _examHallService = examHallService;
        }

        public async Task<ExamHallResponse> Handle(ExamHallGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _examHallService.GetByIdAsync(query.ExamHallId);
        }
    }
}
