using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPoolCandidates.Commands.TalentPoolCandidateCreate
{
    public class TalentPoolCandidateCreateCommand : IRequest<long>
    {
        public TalentPoolCandidateCreateRequest Request { get; set; }

        public TalentPoolCandidateCreateCommand(TalentPoolCandidateCreateRequest request)
        {
            Request = request;
        }
    }
}
