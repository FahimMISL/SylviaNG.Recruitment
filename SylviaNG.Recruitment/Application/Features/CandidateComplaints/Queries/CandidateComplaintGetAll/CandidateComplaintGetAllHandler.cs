using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetAll
{
    public class CandidateComplaintGetAllHandler : IRequestHandler<CandidateComplaintGetAllQuery, List<CandidateComplaintResponse>>
    {
        private readonly ICandidateComplaintService _service;

        public CandidateComplaintGetAllHandler(ICandidateComplaintService service)
        {
            _service = service;
        }

        public async Task<List<CandidateComplaintResponse>> Handle(CandidateComplaintGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
