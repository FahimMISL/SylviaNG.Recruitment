using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateExperiences.Commands.CandidateExperienceCreate
{
    public class CandidateExperienceCreateCommand : IRequest<long>
    {
        public CandidateExperienceCreateRequest Request { get; set; }

        public CandidateExperienceCreateCommand(CandidateExperienceCreateRequest request)
        {
            Request = request;
        }
    }
}
