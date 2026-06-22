using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Commands.ProfileFieldConfigDelete
{
    public class ProfileFieldConfigDeleteCommand : IRequest
    {
        public long Id { get; set; }

        public ProfileFieldConfigDeleteCommand(long id)
        {
            Id = id;
        }
    }
}
