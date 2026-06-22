using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogUpdate
{
    public class ImpersonationLogUpdateCommand : IRequest<Unit>
    {
        public long ImpersonationLogId { get; set; }
        public ImpersonationLogUpdateRequest Request { get; set; }

        public ImpersonationLogUpdateCommand(long impersonationLogId, ImpersonationLogUpdateRequest request)
        {
            ImpersonationLogId = impersonationLogId;
            Request = request;
        }
    }
}
