using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankTalentPoolGetAll
{
    public class CvBankTalentPoolGetAllHandler : IRequestHandler<CvBankTalentPoolGetAllQuery, List<CvBankTalentPoolEntryResponse>>
    {
        private readonly ICandidateTalentPoolRepository _candidateTalentPoolRepository;

        public CvBankTalentPoolGetAllHandler(ICandidateTalentPoolRepository candidateTalentPoolRepository)
        {
            _candidateTalentPoolRepository = candidateTalentPoolRepository;
        }

        public async Task<List<CvBankTalentPoolEntryResponse>> Handle(CvBankTalentPoolGetAllQuery query, CancellationToken cancellationToken)
        {
            var entries = await _candidateTalentPoolRepository.GetAllWithProfileAsync();

            return entries.Select(e => new CvBankTalentPoolEntryResponse
            {
                CandidateProfileId = e.CandidateProfileId,
                FullName = e.CandidateProfile.FullName,
                Email = e.CandidateProfile.Email,
                Phone = e.CandidateProfile.Phone,
                ProfilePhotoPath = e.CandidateProfile.ProfilePhotoPath,
                AddedAt = e.CreatedAt
            }).ToList();
        }
    }
}
