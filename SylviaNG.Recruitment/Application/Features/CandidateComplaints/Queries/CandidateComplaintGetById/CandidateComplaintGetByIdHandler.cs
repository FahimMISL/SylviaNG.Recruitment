using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetById
{
    public class CandidateComplaintGetByIdHandler : IRequestHandler<CandidateComplaintGetByIdQuery, CandidateComplaintResponse>
    {
        private readonly ICandidateComplaintService _service;

        public CandidateComplaintGetByIdHandler(ICandidateComplaintService service)
        {
            _service = service;
        }

        public async Task<CandidateComplaintResponse> Handle(CandidateComplaintGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.CandidateComplaintId);
        }
    }
}
