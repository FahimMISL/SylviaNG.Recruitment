using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;

namespace SylviaNG.Recruitment.Application.Features.Roles.Commands.RoleCreate
{
    public class RoleCreateCommand : IRequest<long>
    {
        public RoleCreateRequest Request { get; set; }

        public RoleCreateCommand(RoleCreateRequest request)
        {
            Request = request;
        }
    }
}
