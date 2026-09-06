using MediatR;
using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferLetters.Queries.OfferLetterGetAll
{
    public class OfferLetterGetAllHandler : IRequestHandler<OfferLetterGetAllQuery, List<OfferLetterResponse>>
    {
        private readonly IOfferLetterService _offerLetterService;

        public OfferLetterGetAllHandler(IOfferLetterService offerLetterService)
        {
            _offerLetterService = offerLetterService;
        }

        public async Task<List<OfferLetterResponse>> Handle(OfferLetterGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _offerLetterService.GetAllAsync(query.JobApplicationId);
        }
    }
}
