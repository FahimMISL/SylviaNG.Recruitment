using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletCreate
{
    public class JoiningBookletCreateCommand : IRequest<long>
    {
        public JoiningBookletCreateRequest Request { get; set; }

        public JoiningBookletCreateCommand(JoiningBookletCreateRequest request)
        {
            Request = request;
        }
    }
}
