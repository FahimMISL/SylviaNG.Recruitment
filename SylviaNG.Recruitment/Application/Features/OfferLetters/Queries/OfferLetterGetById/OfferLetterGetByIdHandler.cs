using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetById
{
    public class OfferLetterGetByIdHandler : IRequestHandler<OfferLetterGetByIdQuery, OfferLetterResponse>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterGetByIdHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<OfferLetterResponse> Handle(OfferLetterGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _offerLetterService.GetByIdAsync(query.OfferLetterId);
        }
    }
}
