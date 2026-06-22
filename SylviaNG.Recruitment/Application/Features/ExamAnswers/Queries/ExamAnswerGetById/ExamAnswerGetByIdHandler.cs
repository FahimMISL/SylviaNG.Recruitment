using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Queries.ExamAnswerGetById
{
    public class ExamAnswerGetByIdHandler : IRequestHandler<ExamAnswerGetByIdQuery, ExamAnswerResponse>
    {
        private readonly IExamAnswerService _service;

        public ExamAnswerGetByIdHandler(IExamAnswerService service)
        {
            _service = service;
        }

        public async Task<ExamAnswerResponse> Handle(ExamAnswerGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ExamAnswerId);
        }
    }
}
