using MediatR;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;

namespace SylviaNG.Recruitment.Application.Features.BloodGroups.Queries.BloodGroupGetAll
{
    public class BloodGroupGetAllQuery : IRequest<List<BloodGroupResponse>>
    {
    }
}
