using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Queries.CandidateEducationGetAll
{
    public class CandidateEducationGetAllHandler : IRequestHandler<CandidateEducationGetAllQuery, List<CandidateEducationResponse>>
    {
        private readonly ICandidateEducationService _service;

        public CandidateEducationGetAllHandler(ICandidateEducationService service)
        {
            _service = service;
        }

        public async Task<List<CandidateEducationResponse>> Handle(CandidateEducationGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
