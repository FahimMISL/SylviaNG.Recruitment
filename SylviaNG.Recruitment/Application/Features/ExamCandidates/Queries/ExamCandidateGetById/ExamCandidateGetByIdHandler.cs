using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetById
{
    public class ExamCandidateGetByIdHandler : IRequestHandler<ExamCandidateGetByIdQuery, ExamCandidateResponse>
    {
        private readonly IExamCandidateService _service;

        public ExamCandidateGetByIdHandler(IExamCandidateService service)
        {
            _service = service;
        }

        public async Task<ExamCandidateResponse> Handle(ExamCandidateGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ExamCandidateId);
        }
    }
}
