using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfilePersonalInfoUpdate
{
    public class CandidateProfilePersonalInfoUpdateCommand : IRequest<Unit>
    {
        public CandidateProfilePersonalInfoUpdateRequest Request { get; set; }

        public CandidateProfilePersonalInfoUpdateCommand(CandidateProfilePersonalInfoUpdateRequest request)
        {
            Request = request;
        }
    }
}
