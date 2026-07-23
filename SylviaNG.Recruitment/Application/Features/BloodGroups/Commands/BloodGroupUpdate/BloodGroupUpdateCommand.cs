using MediatR;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupUpdate
{
    public class BloodGroupUpdateCommand : IRequest<Unit>
    {
        public long BloodGroupId { get; set; }
        public BloodGroupUpdateRequest Request { get; set; }

        public BloodGroupUpdateCommand(long bloodGroupId, BloodGroupUpdateRequest request)
        {
            BloodGroupId = bloodGroupId;
            Request = request;
        }
    }
}
