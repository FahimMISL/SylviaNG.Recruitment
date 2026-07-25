using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.DocumentTracking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DocumentTrackingService : IDocumentTrackingService
    {
        private readonly IOfferLetterRepository _offerLetterRepository;
        private readonly IAppointmentLetterRepository _appointmentLetterRepository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly PortalSettings _portalSettings;

        public DocumentTrackingService(
            IOfferLetterRepository offerLetterRepository,
            IAppointmentLetterRepository appointmentLetterRepository,
            INotificationDispatchService notificationDispatchService,
            IOptions<PortalSettings> portalSettings)
        {
            _offerLetterRepository = offerLetterRepository;
            _appointmentLetterRepository = appointmentLetterRepository;
            _notificationDispatchService = notificationDispatchService;
            _portalSettings = portalSettings.Value;
        }

        public async Task<PagedResult<DocumentTrackingItemResponse>> GetAllAsync(DocumentTrackingFilterRequest filter)
        {
            // Small data scale (see EP-10 F2 plan) - merged in memory rather than a SQL UNION
            // across two different entity shapes.
            var items = new List<DocumentTrackingItemResponse>();

            if (filter.DocumentType is null or DocumentTypeEnum.OfferLetter)
            {
                var offerLetters = await _offerLetterRepository.GetAllOrderedAsync(null);
                items.AddRange(offerLetters.Select(ToTrackingItem));
            }

            if (filter.DocumentType is null or DocumentTypeEnum.AppointmentLetter)
            {
                var appointmentLetters = await _appointmentLetterRepository.GetAllOrderedAsync(null);
                items.AddRange(appointmentLetters.Select(ToTrackingItem));
            }

            if (filter.AcceptanceStatus.HasValue)
                items = items.Where(i => i.AcceptanceStatus == filter.AcceptanceStatus.Value).ToList();

            items = items.OrderByDescending(i => i.GeneratedAt).ToList();

            var totalCount = items.Count;
            var page = items
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResult<DocumentTrackingItemResponse>
            {
                Data = page,
                PageNumber = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
            };
        }

        public async Task FollowUpAsync(DocumentTypeEnum documentType, long sourceId)
        {
            if (documentType != DocumentTypeEnum.OfferLetter)
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(documentType),
                        "A follow-up reminder can only be sent for a pending OfferLetter.")
                });

            var offerLetter = await _offerLetterRepository.GetByIdWithDetailsAsync(sourceId)
                ?? throw new NotFoundException("OfferLetter", sourceId);

            if (offerLetter.Status is not (OfferLetterStatusEnum.Generated or OfferLetterStatusEnum.Sent))
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(sourceId),
                        "This offer letter already has a recorded decision - no follow-up needed.")
                });

            var placeholders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["CandidateName"] = offerLetter.JobApplication.CandidateName,
                ["Designation"] = offerLetter.Designation,
                ["JoiningDate"] = offerLetter.JoiningDate.ToString("dd MMM yyyy"),
                ["PortalLink"] = $"{_portalSettings.FrontendBaseUrl}/candidate-profile/offer-letters",
            };

            await _notificationDispatchService.DispatchAsync(
                RecruitmentEventEnum.OfferLetterAvailable,
                placeholders,
                new NotificationDispatchTargets(offerLetter.JobApplication.CandidateEmail, null, offerLetter.JobApplicationId),
                persistImmediately: true);
        }

        private static DocumentTrackingItemResponse ToTrackingItem(OfferLetter entity) => new()
        {
            DocumentType = DocumentTypeEnum.OfferLetter,
            SourceId = entity.OfferLetterId,
            JobApplicationId = entity.JobApplicationId,
            RecipientName = entity.JobApplication?.CandidateName ?? string.Empty,
            RecipientEmail = entity.JobApplication?.CandidateEmail,
            GeneratedAt = entity.GeneratedAt,
            AcceptanceStatus = entity.Status switch
            {
                OfferLetterStatusEnum.Accepted => DocumentAcceptanceStatusEnum.Accepted,
                OfferLetterStatusEnum.Declined => DocumentAcceptanceStatusEnum.Declined,
                _ => DocumentAcceptanceStatusEnum.Pending,
            },
        };

        private static DocumentTrackingItemResponse ToTrackingItem(AppointmentLetter entity) => new()
        {
            DocumentType = DocumentTypeEnum.AppointmentLetter,
            SourceId = entity.AppointmentLetterId,
            JobApplicationId = entity.JobApplicationId,
            RecipientName = entity.JobApplication?.CandidateName ?? string.Empty,
            RecipientEmail = entity.JobApplication?.CandidateEmail,
            GeneratedAt = entity.GeneratedAt,
            AcceptanceStatus = DocumentAcceptanceStatusEnum.NotApplicable,
        };
    }
}
