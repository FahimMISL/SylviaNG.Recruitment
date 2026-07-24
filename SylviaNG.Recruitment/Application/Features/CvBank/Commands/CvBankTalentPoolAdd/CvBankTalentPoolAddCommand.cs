using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolAdd
{
    public class CvBankTalentPoolAddCommand : IRequest<CvBankTalentPoolAddResponse>
    {
        public CvBankTalentPoolAddRequest Request { get; }

        public CvBankTalentPoolAddCommand(CvBankTalentPoolAddRequest request)
        {
            Request = request;
        }
    }
}
