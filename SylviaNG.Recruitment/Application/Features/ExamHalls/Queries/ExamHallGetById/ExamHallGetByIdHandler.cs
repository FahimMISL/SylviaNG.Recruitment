using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamHalls.Queries.ExamHallGetById
{
    public class ExamHallGetByIdHandler : IRequestHandler<ExamHallGetByIdQuery, ExamHallResponse>
    {
        private readonly IExamHallService _service;

        public ExamHallGetByIdHandler(IExamHallService service)
        {
            _service = service;
        }

        public async Task<ExamHallResponse> Handle(ExamHallGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ExamHallId);
        }
    }
}
