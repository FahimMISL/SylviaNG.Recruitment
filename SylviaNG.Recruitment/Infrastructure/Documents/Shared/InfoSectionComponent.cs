using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>EP-18 F1: generic labeled section for structured info (exam details, salary
    /// summary, etc) - reusable across any document that needs a titled block of label/value
    /// pairs.</summary>
    public class InfoSectionComponent : IComponent
    {
        private readonly CompanyBranding _branding;
        private readonly string _sectionTitle;
        private readonly IReadOnlyList<(string Label, string Value)> _fields;

        public InfoSectionComponent(CompanyBranding branding, string sectionTitle, IReadOnlyList<(string Label, string Value)> fields)
        {
            _branding = branding;
            _sectionTitle = sectionTitle;
            _fields = fields;
        }

        public void Compose(IContainer container)
        {
            var primaryColor = DocumentFrame.ResolveColor(_branding.PrimaryColor, Colors.Brown.Darken2);

            container.Column(column =>
            {
                column.Spacing(4);

                if (!string.IsNullOrWhiteSpace(_sectionTitle))
                {
                    column.Item().Text(_sectionTitle).Bold().FontSize(10).FontColor(primaryColor);
                }

                foreach (var field in _fields)
                {
                    column.Item().Text(text =>
                    {
                        text.Span($"{field.Label}: ").SemiBold();
                        text.Span(field.Value);
                    });
                }
            });
        }
    }
}
