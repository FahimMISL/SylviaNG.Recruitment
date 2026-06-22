using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetAllPaged
{
    public class CandidateComplaintGetAllPagedHandler : IRequestHandler<CandidateComplaintGetAllPagedQuery, PagedResult<CandidateComplaintResponse>>
    {
        private readonly ICandidateComplaintService _candidateComplaintService;

        public CandidateComplaintGetAllPagedHandler(ICandidateComplaintService candidateComplaintService)
        {
            _candidateComplaintService = candidateComplaintService;
        }

        public async Task<PagedResult<CandidateComplaintResponse>> Handle(CandidateComplaintGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _candidateComplaintService.GetPaginatedAsync(query.Request);
        }
    }
}
