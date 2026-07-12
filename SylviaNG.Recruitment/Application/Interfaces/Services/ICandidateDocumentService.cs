using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ICandidateDocumentService
    {
        Task<List<CandidateDocumentResponse>> GetAllForCurrentCandidateAsync();
        Task<CandidateDocumentResponse> UploadAsync(CandidateDocumentUploadRequest request);
        Task<CandidateDocumentResponse> UpdateAsync(long candidateDocumentId, CandidateDocumentUpdateRequest request);
        Task DeleteAsync(long candidateDocumentId);
    }
}
