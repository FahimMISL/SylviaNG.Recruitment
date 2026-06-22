using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletUpdate
{
    public class JoiningBookletUpdateCommand : IRequest<Unit>
    {
        public long JoiningBookletId { get; set; }
        public JoiningBookletUpdateRequest Request { get; set; }

        public JoiningBookletUpdateCommand(long joiningBookletId, JoiningBookletUpdateRequest request)
        {
            JoiningBookletId = joiningBookletId;
            Request = request;
        }
    }
}
