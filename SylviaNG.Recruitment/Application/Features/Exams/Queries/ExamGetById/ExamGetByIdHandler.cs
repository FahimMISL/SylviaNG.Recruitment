using MediatR;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Exams.Queries.ExamGetById
{
    public class ExamGetByIdHandler : IRequestHandler<ExamGetByIdQuery, ExamResponse>
    {
        private readonly IExamService _examService;

        public ExamGetByIdHandler(IExamService examService)
        {
            _examService = examService;
        }

        public async Task<ExamResponse> Handle(ExamGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _examService.GetByIdAsync(query.ExamId);
        }
    }
}
