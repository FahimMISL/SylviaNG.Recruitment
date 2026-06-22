using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Queries.ProfileFieldConfigGetById
{
    public class ProfileFieldConfigGetByIdQuery : IRequest<ProfileFieldConfigResponse>
    {
        public long Id { get; set; }

        public ProfileFieldConfigGetByIdQuery(long id)
        {
            Id = id;
        }
    }
}
