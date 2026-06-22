using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetById
{
    public class QuestionOptionGetByIdHandler : IRequestHandler<QuestionOptionGetByIdQuery, QuestionOptionResponse>
    {
        private readonly IQuestionOptionService _service;

        public QuestionOptionGetByIdHandler(IQuestionOptionService service)
        {
            _service = service;
        }

        public async Task<QuestionOptionResponse> Handle(QuestionOptionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.QuestionOptionId);
        }
    }
}
