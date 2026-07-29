using MediatR;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolAdd
{
    public class CvBankTalentPoolAddHandler : IRequestHandler<CvBankTalentPoolAddCommand, CvBankTalentPoolAddResponse>
    {
        private readonly ICandidateTalentPoolRepository _candidateTalentPoolRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CvBankTalentPoolAddHandler(ICandidateTalentPoolRepository candidateTalentPoolRepository, IUnitOfWork unitOfWork)
        {
            _candidateTalentPoolRepository = candidateTalentPoolRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CvBankTalentPoolAddResponse> Handle(CvBankTalentPoolAddCommand command, CancellationToken cancellationToken)
        {
            var requestedIds = command.Request.CandidateProfileIds.Distinct().ToList();
            var existingIds = await _candidateTalentPoolRepository.GetExistingCandidateProfileIdsAsync(requestedIds);

            var toAdd = requestedIds.Where(id => !existingIds.Contains(id)).ToList();

            foreach (var candidateProfileId in toAdd)
            {
                await _candidateTalentPoolRepository.AddAsync(new CandidateTalentPool
                {
                    CandidateProfileId = candidateProfileId
                });
            }

            if (toAdd.Count > 0)
                await _unitOfWork.SaveChangesAsync();

            return new CvBankTalentPoolAddResponse
            {
                AddedCount = toAdd.Count,
                AlreadyInPoolCount = existingIds.Count
            };
        }
    }
}
