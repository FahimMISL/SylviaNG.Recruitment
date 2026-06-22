using MediatR;
using SylviaNG.Recruitment.Application.Features.Questions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Questions.Queries.QuestionGetAll
{
    public class QuestionGetAllHandler : IRequestHandler<QuestionGetAllQuery, List<QuestionResponse>>
    {
        private readonly IQuestionService _service;

        public QuestionGetAllHandler(IQuestionService service)
        {
            _service = service;
        }

        public async Task<List<QuestionResponse>> Handle(QuestionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
