using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterAccept
{
    public class OfferLetterAcceptHandler : IRequestHandler<OfferLetterAcceptCommand, OfferLetterResponse>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterAcceptHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<OfferLetterResponse> Handle(OfferLetterAcceptCommand command, CancellationToken cancellationToken)
        {
            return await _offerLetterService.AcceptAsync(command.OfferLetterId);
        }
    }
}
