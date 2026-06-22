using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintDelete
{
    public class CandidateComplaintDeleteCommand : IRequest<Unit>
    {
        public long CandidateComplaintId { get; set; }

        public CandidateComplaintDeleteCommand(long candidateComplaintId)
        {
            CandidateComplaintId = candidateComplaintId;
        }
    }
}
