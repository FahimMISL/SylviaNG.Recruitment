using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetAll
{
    public class ProfileFieldConfigGetAllQuery : IRequest<List<ProfileFieldConfigResponse>>
    {
    }
}
