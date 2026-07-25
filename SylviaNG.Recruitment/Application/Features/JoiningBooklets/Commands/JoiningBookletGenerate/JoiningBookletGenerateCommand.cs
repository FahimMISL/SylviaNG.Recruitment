using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletGenerate
{
    public class JoiningBookletGenerateCommand : IRequest<JoiningBookletResponse>
    {
        public JoiningBookletGenerateRequest Request { get; set; }

        public JoiningBookletGenerateCommand(JoiningBookletGenerateRequest request)
        {
            Request = request;
        }
    }
}
