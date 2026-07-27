using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreateBulkCvZip
{
    public class ExportRequestCreateBulkCvZipCommand : IRequest<long>
    {
        public List<long> JobApplicationIds { get; }

        public ExportRequestCreateBulkCvZipCommand(List<long> jobApplicationIds)
        {
            JobApplicationIds = jobApplicationIds;
        }
    }
}
