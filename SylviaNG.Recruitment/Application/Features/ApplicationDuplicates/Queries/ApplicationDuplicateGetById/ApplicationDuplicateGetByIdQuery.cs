using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetById
{
    public class ApplicationDuplicateGetByIdQuery : IRequest<ApplicationDuplicateResponse>
    {
        public long ApplicationDuplicateId { get; set; }

        public ApplicationDuplicateGetByIdQuery(long applicationDuplicateId)
        {
            ApplicationDuplicateId = applicationDuplicateId;
        }
    }
}
