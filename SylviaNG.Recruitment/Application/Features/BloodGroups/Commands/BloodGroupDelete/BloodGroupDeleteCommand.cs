using MediatR;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupDelete
{
    public class BloodGroupDeleteCommand : IRequest<Unit>
    {
        public long BloodGroupId { get; set; }

        public BloodGroupDeleteCommand(long bloodGroupId)
        {
            BloodGroupId = bloodGroupId;
        }
    }
}
