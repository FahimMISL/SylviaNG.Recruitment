using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolFastTrack
{
    public class TalentPoolFastTrackHandler : IRequestHandler<TalentPoolFastTrackCommand, TalentPoolFastTrackResponse>
    {
        private readonly ITalentPoolService _talentPoolService;

        public TalentPoolFastTrackHandler(ITalentPoolService talentPoolService)
        {
            _talentPoolService = talentPoolService;
        }

        public async Task<TalentPoolFastTrackResponse> Handle(TalentPoolFastTrackCommand command, CancellationToken cancellationToken)
        {
            return await _talentPoolService.FastTrackAsync(command.Request);
        }
    }
}
