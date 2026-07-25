using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Commands.OfferLetterGenerate
{
    public class OfferLetterGenerateHandler : IRequestHandler<OfferLetterGenerateCommand, OfferLetterResponse>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterGenerateHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<OfferLetterResponse> Handle(OfferLetterGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _offerLetterService.GenerateAsync(command.Request);
        }
    }
}
