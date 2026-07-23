using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Queries.ExamQuestionGetById
{
    public class ExamQuestionGetByIdHandler : IRequestHandler<ExamQuestionGetByIdQuery, ExamQuestionResponse>
    {
        private readonly IExamQuestionService _examQuestionService;

        public ExamQuestionGetByIdHandler(IExamQuestionService examQuestionService)
        {
            _examQuestionService = examQuestionService;
        }

        public async Task<ExamQuestionResponse> Handle(ExamQuestionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _examQuestionService.GetByIdAsync(query.ExamQuestionId);
        }
    }
}
