using MediatR;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Queries.ExamCandidateGetAll
{
    public class ExamCandidateGetAllHandler : IRequestHandler<ExamCandidateGetAllQuery, List<ExamCandidateResponse>>
    {
        private readonly IExamCandidateService _service;

        public ExamCandidateGetAllHandler(IExamCandidateService service)
        {
            _service = service;
        }

        public async Task<List<ExamCandidateResponse>> Handle(ExamCandidateGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
