using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.CandidateProfileHrNotesUpdate
{
    public class CandidateProfileHrNotesUpdateCommand : IRequest<Unit>
    {
        public long CandidateProfileId { get; set; }
        public string? HrNotes { get; set; }

        public CandidateProfileHrNotesUpdateCommand(long candidateProfileId, string? hrNotes)
        {
            CandidateProfileId = candidateProfileId;
            HrNotes = hrNotes;
        }
    }
}
