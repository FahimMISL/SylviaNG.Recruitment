using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupUpdate
{
    public class BloodGroupUpdateHandler : IRequestHandler<BloodGroupUpdateCommand, Unit>
    {
        private readonly IBloodGroupService _genderService;

        public BloodGroupUpdateHandler(IBloodGroupService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(BloodGroupUpdateCommand command, CancellationToken cancellationToken)
        {
            await _genderService.UpdateAsync(command.BloodGroupId, command.Request);
            return Unit.Value;
        }
    }
}
