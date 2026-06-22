using MediatR;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Queries.QuestionOptionGetAll
{
    public class QuestionOptionGetAllHandler : IRequestHandler<QuestionOptionGetAllQuery, List<QuestionOptionResponse>>
    {
        private readonly IQuestionOptionService _service;

        public QuestionOptionGetAllHandler(IQuestionOptionService service)
        {
            _service = service;
        }

        public async Task<List<QuestionOptionResponse>> Handle(QuestionOptionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
