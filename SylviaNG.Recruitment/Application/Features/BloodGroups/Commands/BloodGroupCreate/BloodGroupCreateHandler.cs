using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupCreate
{
    public class BloodGroupCreateHandler : IRequestHandler<BloodGroupCreateCommand, long>
    {
        private readonly IBloodGroupService _genderService;

        public BloodGroupCreateHandler(IBloodGroupService genderService)
        {
            _genderService = genderService;
        }

        public async Task<long> Handle(BloodGroupCreateCommand command, CancellationToken cancellationToken)
        {
            return await _genderService.CreateAsync(command.Request);
        }
    }
}
