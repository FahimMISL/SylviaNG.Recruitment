using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintUpdate
{
    public class CandidateComplaintUpdateCommand : IRequest<Unit>
    {
        public long CandidateComplaintId { get; set; }
        public CandidateComplaintUpdateRequest Request { get; set; }

        public CandidateComplaintUpdateCommand(long candidateComplaintId, CandidateComplaintUpdateRequest request)
        {
            CandidateComplaintId = candidateComplaintId;
            Request = request;
        }
    }
}
