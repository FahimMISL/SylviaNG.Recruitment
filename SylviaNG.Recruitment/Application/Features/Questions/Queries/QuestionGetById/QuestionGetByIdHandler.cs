using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetById
{
    public class QuestionGetByIdHandler : IRequestHandler<QuestionGetByIdQuery, QuestionResponse>
    {
        private readonly IQuestionService _service;

        public QuestionGetByIdHandler(IQuestionService service)
        {
            _service = service;
        }

        public async Task<QuestionResponse> Handle(QuestionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.QuestionId);
        }
    }
}
