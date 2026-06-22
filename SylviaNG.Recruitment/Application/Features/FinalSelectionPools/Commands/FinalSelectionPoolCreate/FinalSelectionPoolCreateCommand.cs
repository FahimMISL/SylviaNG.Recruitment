using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Commands.FinalSelectionPoolCreate
{
    public class FinalSelectionPoolCreateCommand : IRequest<long>
    {
        public FinalSelectionPoolCreateRequest Request { get; set; }

        public FinalSelectionPoolCreateCommand(FinalSelectionPoolCreateRequest request)
        {
            Request = request;
        }
    }
}
