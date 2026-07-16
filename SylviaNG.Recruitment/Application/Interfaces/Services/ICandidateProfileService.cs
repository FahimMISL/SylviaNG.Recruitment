using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateProfileService
    {
        Task<CandidateProfileResponse> GetMyProfileAsync();
        Task UpdatePersonalInfoAsync(CandidateProfilePersonalInfoUpdateRequest request);
        Task UpdateContactAsync(CandidateProfileContactUpdateRequest request);
        Task<string> UploadPhotoAsync(IFormFile file);
        Task DeletePhotoAsync();
        Task<string> UploadSignatureAsync(IFormFile file);
        Task DeleteSignatureAsync();

        /// <summary>Paged, searchable candidate list for the HR/Admin view (US-009). Optionally
        /// narrowed to candidates having any of the given tags (US-041 AC3).</summary>
        Task<PagedResult<CandidateProfileSummaryResponse>> GetPagedAsync(PagedRequest request, List<string>? tags = null);

        /// <summary>Full read-only aggregate of one candidate's profile, for HR/Admin (US-009).</summary>
        Task<CandidateProfileDetailResponse> GetProfileDetailAsync(long candidateProfileId);

        /// <summary>HR/Admin-only annotation on a candidate's profile (US-009 AC5).</summary>
        Task UpdateHrNotesAsync(long candidateProfileId, string? hrNotes);
    }
}
