using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterDecline
{
    public class OfferLetterDeclineHandler : IRequestHandler<OfferLetterDeclineCommand, OfferLetterResponse>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterDeclineHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<OfferLetterResponse> Handle(OfferLetterDeclineCommand command, CancellationToken cancellationToken)
        {
            return await _offerLetterService.DeclineAsync(command.OfferLetterId, command.Request.Reason);
        }
    }
}
