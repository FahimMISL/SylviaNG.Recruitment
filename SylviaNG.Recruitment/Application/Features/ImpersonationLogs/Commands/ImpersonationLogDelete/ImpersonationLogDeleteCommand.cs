using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Commands.ImpersonationLogDelete
{
    public class ImpersonationLogDeleteCommand : IRequest<Unit>
    {
        public long ImpersonationLogId { get; set; }

        public ImpersonationLogDeleteCommand(long impersonationLogId)
        {
            ImpersonationLogId = impersonationLogId;
        }
    }
}
