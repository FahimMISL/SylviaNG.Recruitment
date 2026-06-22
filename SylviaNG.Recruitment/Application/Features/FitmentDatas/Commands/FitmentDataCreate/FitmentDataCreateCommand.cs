using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataCreate
{
    public class FitmentDataCreateCommand : IRequest<long>
    {
        public FitmentDataCreateRequest Request { get; set; }

        public FitmentDataCreateCommand(FitmentDataCreateRequest request)
        {
            Request = request;
        }
    }
}
