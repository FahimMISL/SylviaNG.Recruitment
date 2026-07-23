using MediatR;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Commands.BloodGroupCreate
{
    public class BloodGroupCreateCommand : IRequest<long>
    {
        public BloodGroupCreateRequest Request { get; set; }

        public BloodGroupCreateCommand(BloodGroupCreateRequest request)
        {
            Request = request;
        }
    }
}
