using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Commands.ApplicationScreeningResultCreate
{
    public class ApplicationScreeningResultCreateCommand : IRequest<long>
    {
        public ApplicationScreeningResultCreateRequest Request { get; set; }

        public ApplicationScreeningResultCreateCommand(ApplicationScreeningResultCreateRequest request)
        {
            Request = request;
        }
    }
}
