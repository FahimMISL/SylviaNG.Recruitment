using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigUpdate
{
    public class ProfileFieldConfigUpdateCommand : IRequest
    {
        public long Id { get; set; }
        public ProfileFieldConfigUpdateRequest Request { get; set; }

        public ProfileFieldConfigUpdateCommand(long id, ProfileFieldConfigUpdateRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
