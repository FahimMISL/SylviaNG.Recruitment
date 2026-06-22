using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogCreate
{
    public class ImpersonationLogCreateCommand : IRequest<long>
    {
        public ImpersonationLogCreateRequest Request { get; set; }

        public ImpersonationLogCreateCommand(ImpersonationLogCreateRequest request)
        {
            Request = request;
        }
    }
}
