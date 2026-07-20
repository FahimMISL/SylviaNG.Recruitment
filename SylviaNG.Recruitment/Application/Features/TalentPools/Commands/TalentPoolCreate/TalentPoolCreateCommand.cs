using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCreate
{
    public class TalentPoolCreateCommand : IRequest<long>
    {
        public TalentPoolCreateRequest Request { get; set; }

        public TalentPoolCreateCommand(TalentPoolCreateRequest request)
        {
            Request = request;
        }
    }
}
