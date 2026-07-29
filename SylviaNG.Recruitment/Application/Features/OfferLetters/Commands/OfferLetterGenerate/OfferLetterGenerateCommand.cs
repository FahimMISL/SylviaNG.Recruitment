using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterGenerate
{
    public class OfferLetterGenerateCommand : IRequest<OfferLetterResponse>
    {
        public OfferLetterGenerateRequest Request { get; set; }

        public OfferLetterGenerateCommand(OfferLetterGenerateRequest request)
        {
            Request = request;
        }
    }
}
