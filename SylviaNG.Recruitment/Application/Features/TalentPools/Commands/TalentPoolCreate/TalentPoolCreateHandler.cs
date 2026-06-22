using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Commands.TalentPoolCreate
{
    public class TalentPoolCreateHandler : IRequestHandler<TalentPoolCreateCommand, long>
    {
        private readonly ITalentPoolService _service;

        public TalentPoolCreateHandler(ITalentPoolService service)
        {
            _service = service;
        }

        public async Task<long> Handle(TalentPoolCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
