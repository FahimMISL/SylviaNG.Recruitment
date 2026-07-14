using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateWorkExperienceCreate
{
    public class CandidateWorkExperienceCreateCommand : IRequest<long>
    {
        public CandidateWorkExperienceCreateRequest Request { get; set; }

        public CandidateWorkExperienceCreateCommand(CandidateWorkExperienceCreateRequest request)
        {
            Request = request;
        }
    }
}
