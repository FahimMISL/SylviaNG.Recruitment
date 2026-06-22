using MediatR;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletDelete
{
    public class JoiningBookletDeleteCommand : IRequest<Unit>
    {
        public long JoiningBookletId { get; set; }

        public JoiningBookletDeleteCommand(long joiningBookletId)
        {
            JoiningBookletId = joiningBookletId;
        }
    }
}
