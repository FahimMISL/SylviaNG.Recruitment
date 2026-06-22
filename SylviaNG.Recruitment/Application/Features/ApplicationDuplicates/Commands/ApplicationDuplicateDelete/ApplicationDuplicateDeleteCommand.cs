using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateDelete
{
    public class ApplicationDuplicateDeleteCommand : IRequest<Unit>
    {
        public long ApplicationDuplicateId { get; set; }

        public ApplicationDuplicateDeleteCommand(long applicationDuplicateId)
        {
            ApplicationDuplicateId = applicationDuplicateId;
        }
    }
}
