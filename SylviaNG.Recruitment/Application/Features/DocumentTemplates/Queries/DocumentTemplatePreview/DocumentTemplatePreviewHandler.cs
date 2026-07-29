using MediatR;
using SylviaNG.Recruitment.Application.Features.DocumentTemplates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Queries.DocumentTemplatePreview
{
    public class DocumentTemplatePreviewHandler : IRequestHandler<DocumentTemplatePreviewQuery, DocumentTemplatePreviewResponse>
    {
        private readonly IPlaceholderSubstitutionService _placeholderSubstitutionService;

        public DocumentTemplatePreviewHandler(IPlaceholderSubstitutionService placeholderSubstitutionService)
        {
            _placeholderSubstitutionService = placeholderSubstitutionService;
        }

        public Task<DocumentTemplatePreviewResponse> Handle(DocumentTemplatePreviewQuery query, CancellationToken cancellationToken)
        {
            var request = query.Request;

            var response = new DocumentTemplatePreviewResponse
            {
                RenderedBody = _placeholderSubstitutionService.Render(request.Body, request.PlaceholderValues),
                DetectedPlaceholders = _placeholderSubstitutionService.ExtractPlaceholders(request.Body),
            };

            return Task.FromResult(response);
        }
    }
}
