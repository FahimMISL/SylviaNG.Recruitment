using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupDelete
{
    public class BloodGroupDeleteHandler : IRequestHandler<BloodGroupDeleteCommand, Unit>
    {
        private readonly IBloodGroupService _genderService;

        public BloodGroupDeleteHandler(IBloodGroupService genderService)
        {
            _genderService = genderService;
        }

        public async Task<Unit> Handle(BloodGroupDeleteCommand command, CancellationToken cancellationToken)
        {
            await _genderService.DeleteAsync(command.BloodGroupId);
            return Unit.Value;
        }
    }
}
