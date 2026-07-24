using MediatR;
using SylviaNG.Recruitment.Application.Features.NotificationTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.NotificationTemplates.Queries.NotificationTemplatePreview
{
    public class NotificationTemplatePreviewHandler : IRequestHandler<NotificationTemplatePreviewQuery, NotificationTemplatePreviewResponse>
    {
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;

        public NotificationTemplatePreviewHandler(IPlaceholderSubstitutionService placeholderSubstitutionService)
        {
            _placeholderSubstitutionService = placeholderSubstitutionService;
        }

        public Task<NotificationTemplatePreviewResponse> Handle(NotificationTemplatePreviewQuery query, CancellationToken cancellationToken)
        {
            var request = query.Request;

            var detectedInBody = _placeholderSubstitutionService.ExtractPlaceholders(request.Body);
            var detectedInSubject = string.IsNullOrEmpty(request.Subject)
                ? new List<string>()
                : _placeholderSubstitutionService.ExtractPlaceholders(request.Subject);
            var detected = detectedInSubject.Concat(detectedInBody).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var response = new NotificationTemplatePreviewResponse
            {
                RenderedSubject = string.IsNullOrEmpty(request.Subject)
                    ? request.Subject
                    : _placeholderSubstitutionService.Render(request.Subject, request.PlaceholderValues),
                RenderedBody = _placeholderSubstitutionService.Render(request.Body, request.PlaceholderValues),
                DetectedPlaceholders = detected,
            };

            return Task.FromResult(response);
        }
    }
}
