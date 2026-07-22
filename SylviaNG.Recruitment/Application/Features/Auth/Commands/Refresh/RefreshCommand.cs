using MediatR;
using SylviaNG.Recruitment.Application.Features.Auth.Models;

namespace SylviaNG.Recruitment.Application.Features.Auth.Commands.Refresh
{
    public class RefreshCommand : IRequest<LoginResponse>
    {
        public RefreshRequest Request { get; set; }

        public RefreshCommand(RefreshRequest request)
        {
            Request = request;
        }
    }
}
