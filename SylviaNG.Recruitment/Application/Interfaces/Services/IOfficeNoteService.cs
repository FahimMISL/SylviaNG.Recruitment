using SylviaNG.Recruitment.Application.Features.OfficeNotes.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IOfficeNoteService
    {
        /// <summary>Pre-generate checklist of which enclosures (offer letter/appointment letter/
        /// joining booklet) exist for this application, so HR can see what will be included before
        /// generating.</summary>
        Task<OfficeNoteEnclosuresResponse> GetEnclosuresAsync(long jobApplicationId);

        Task<OfficeNoteResponse> GenerateAsync(OfficeNoteGenerateRequest request);
        Task<List<OfficeNoteResponse>> GetAllAsync(long? jobApplicationId);
        Task<OfficeNoteResponse> GetByIdAsync(long officeNoteId);
    }
}
