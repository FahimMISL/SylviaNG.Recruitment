using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetAll
{
    public class ExamAnswerGetAllHandler : IRequestHandler<ExamAnswerGetAllQuery, List<ExamAnswerResponse>>
    {
        private readonly IExamAnswerService _service;

        public ExamAnswerGetAllHandler(IExamAnswerService service)
        {
            _service = service;
        }

        public async Task<List<ExamAnswerResponse>> Handle(ExamAnswerGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
