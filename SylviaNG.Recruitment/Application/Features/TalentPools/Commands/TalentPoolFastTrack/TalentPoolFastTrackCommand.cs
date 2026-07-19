using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolFastTrack
{
    public class TalentPoolFastTrackCommand : IRequest<TalentPoolFastTrackResponse>
    {
        public TalentPoolFastTrackRequest Request { get; set; }

        public TalentPoolFastTrackCommand(TalentPoolFastTrackRequest request)
        {
            Request = request;
        }
    }
}
