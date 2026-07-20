using MediatR;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolRemove
{
    public class CvBankTalentPoolRemoveHandler : IRequestHandler<CvBankTalentPoolRemoveCommand>
    {
        private readonly ICandidateTalentPoolRepository _candidateTalentPoolRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CvBankTalentPoolRemoveHandler(ICandidateTalentPoolRepository candidateTalentPoolRepository, IUnitOfWork unitOfWork)
        {
            _candidateTalentPoolRepository = candidateTalentPoolRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CvBankTalentPoolRemoveCommand command, CancellationToken cancellationToken)
        {
            var entity = await _candidateTalentPoolRepository.GetByCandidateProfileIdAsync(command.CandidateProfileId)
                ?? throw new NotFoundException("CandidateTalentPool", command.CandidateProfileId);

            _candidateTalentPoolRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
