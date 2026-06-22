using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Commands.ApplicationDuplicateCreate
{
    public class ApplicationDuplicateCreateCommand : IRequest<long>
    {
        public ApplicationDuplicateCreateRequest Request { get; set; }

        public ApplicationDuplicateCreateCommand(ApplicationDuplicateCreateRequest request)
        {
            Request = request;
        }
    }
}
