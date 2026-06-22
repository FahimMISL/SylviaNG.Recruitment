using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateUpdate
{
    public class ApplicationDuplicateUpdateCommand : IRequest<Unit>
    {
        public long ApplicationDuplicateId { get; set; }
        public ApplicationDuplicateUpdateRequest Request { get; set; }

        public ApplicationDuplicateUpdateCommand(long applicationDuplicateId, ApplicationDuplicateUpdateRequest request)
        {
            ApplicationDuplicateId = applicationDuplicateId;
            Request = request;
        }
    }
}
