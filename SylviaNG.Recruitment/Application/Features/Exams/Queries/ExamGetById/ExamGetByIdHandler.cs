using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetById
{
    public class ExamGetByIdHandler : IRequestHandler<ExamGetByIdQuery, ExamResponse>
    {
        private readonly IExamService _service;

        public ExamGetByIdHandler(IExamService service)
        {
            _service = service;
        }

        public async Task<ExamResponse> Handle(ExamGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ExamId);
        }
    }
}
