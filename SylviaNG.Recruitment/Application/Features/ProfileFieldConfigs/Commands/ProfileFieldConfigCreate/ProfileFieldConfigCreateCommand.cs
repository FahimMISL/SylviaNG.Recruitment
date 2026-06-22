using MediatR;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigCreate
{
    public class ProfileFieldConfigCreateCommand : IRequest<long>
    {
        public ProfileFieldConfigCreateRequest Request { get; set; }

        public ProfileFieldConfigCreateCommand(ProfileFieldConfigCreateRequest request)
        {
            Request = request;
        }
    }
}
