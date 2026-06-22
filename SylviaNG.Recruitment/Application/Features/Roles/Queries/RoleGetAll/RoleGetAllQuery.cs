using MediatR;
using SylviaNG.Recruitment.Application.Features.Roles.Models;

namespace SylviaNG.Recruitment.Application.Features.Roles.Queries.RoleGetAll
{
    public class RoleGetAllQuery : IRequest<List<RoleResponse>>
    {
    }
}
