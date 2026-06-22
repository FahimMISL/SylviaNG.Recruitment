using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetAll
{
    public class ExamGetAllHandler : IRequestHandler<ExamGetAllQuery, List<ExamResponse>>
    {
        private readonly IExamService _service;

        public ExamGetAllHandler(IExamService service)
        {
            _service = service;
        }

        public async Task<List<ExamResponse>> Handle(ExamGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
