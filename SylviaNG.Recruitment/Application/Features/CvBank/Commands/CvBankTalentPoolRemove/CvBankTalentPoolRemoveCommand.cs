using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolRemove
{
    public class CvBankTalentPoolRemoveCommand : IRequest
    {
        public long CandidateProfileId { get; }

        public CvBankTalentPoolRemoveCommand(long candidateProfileId)
        {
            CandidateProfileId = candidateProfileId;
        }
    }
}
