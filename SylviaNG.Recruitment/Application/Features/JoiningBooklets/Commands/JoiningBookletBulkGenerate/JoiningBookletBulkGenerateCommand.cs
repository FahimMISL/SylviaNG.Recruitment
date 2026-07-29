using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Commands.JoiningBookletBulkGenerate
{
    public class JoiningBookletBulkGenerateCommand : IRequest<JoiningBookletBulkGenerateResponse>
    {
        public JoiningBookletBulkGenerateRequest Request { get; set; }

        public JoiningBookletBulkGenerateCommand(JoiningBookletBulkGenerateRequest request)
        {
            Request = request;
        }
    }
}
