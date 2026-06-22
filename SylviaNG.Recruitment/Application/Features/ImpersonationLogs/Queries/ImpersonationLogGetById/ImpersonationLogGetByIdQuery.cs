using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetById
{
    public class ImpersonationLogGetByIdQuery : IRequest<ImpersonationLogResponse>
    {
        public long ImpersonationLogId { get; set; }

        public ImpersonationLogGetByIdQuery(long impersonationLogId)
        {
            ImpersonationLogId = impersonationLogId;
        }
    }
}
