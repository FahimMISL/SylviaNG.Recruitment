using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateComplaints.Commands.CandidateComplaintCreate
{
    public class CandidateComplaintCreateCommand : IRequest<long>
    {
        public CandidateComplaintCreateRequest Request { get; set; }

        public CandidateComplaintCreateCommand(CandidateComplaintCreateRequest request)
        {
            Request = request;
        }
    }
}
