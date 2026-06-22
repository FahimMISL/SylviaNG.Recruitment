using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Queries.CandidateComplaintGetById
{
    public class CandidateComplaintGetByIdQuery : IRequest<CandidateComplaintResponse>
    {
        public long CandidateComplaintId { get; set; }

        public CandidateComplaintGetByIdQuery(long candidateComplaintId)
        {
            CandidateComplaintId = candidateComplaintId;
        }
    }
}
