using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetEligibleCandidates
{
    public class JoiningBookletGetEligibleCandidatesHandler : IRequestHandler<JoiningBookletGetEligibleCandidatesQuery, List<JoiningBookletEligibleCandidateResponse>>
    {
        private readonly IJoiningBookletService _joiningBookletService;

        public JoiningBookletGetEligibleCandidatesHandler(IJoiningBookletService joiningBookletService)
        {
            _joiningBookletService = joiningBookletService;
        }

        public async Task<List<JoiningBookletEligibleCandidateResponse>> Handle(JoiningBookletGetEligibleCandidatesQuery query, CancellationToken cancellationToken)
        {
            return await _joiningBookletService.GetEligibleCandidatesAsync();
        }
    }
}
