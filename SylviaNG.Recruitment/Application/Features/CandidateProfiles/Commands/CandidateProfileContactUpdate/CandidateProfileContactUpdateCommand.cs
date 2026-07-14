using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileContactUpdate
{
    public class CandidateProfileContactUpdateCommand : IRequest<Unit>
    {
        public CandidateProfileContactUpdateRequest Request { get; set; }

        public CandidateProfileContactUpdateCommand(CandidateProfileContactUpdateRequest request)
        {
            Request = request;
        }
    }
}
