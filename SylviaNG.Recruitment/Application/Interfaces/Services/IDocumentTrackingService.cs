using SylviaNG.Recruitment.Application.Features.DocumentTracking.Models;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IDocumentTrackingService
    {
        Task<PagedResult<DocumentTrackingItemResponse>> GetAllAsync(DocumentTrackingFilterRequest filter);

        // AC3: re-dispatch the "offer available" reminder. Only meaningful for a Pending
        // OfferLetter - AppointmentLetter has no candidate-decision step to follow up on.
        Task FollowUpAsync(DocumentTypeEnum documentType, long sourceId);
    }
}
