using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>EP-18 F1: shared document footer - company contact info and, if enabled, a page
    /// number, arranged per CompanyBranding.FooterLayout.</summary>
    public class DocumentFooterComponent : IComponent
    {
        private readonly CompanyBranding _branding;

        public DocumentFooterComponent(CompanyBranding branding)
        {
            _branding = branding;
        }

        public void Compose(IContainer container)
        {
            var accentColor = DocumentFrame.ResolveColor(_branding.AccentColor, Colors.Brown.Lighten4);

            container.Column(column =>
            {
                if (_branding.FooterDividerStyle != DocumentDividerStyleEnum.None)
                {
                    column.Item().PaddingBottom(4).LineHorizontal(1).LineColor(accentColor);
                }

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        var contactParts = new[] { _branding.AddressLine, _branding.Phone, _branding.Email, _branding.Website }
                            .Where(part => !string.IsNullOrWhiteSpace(part));

                        text.Span(string.Join("  |  ", contactParts)).FontSize(8).FontColor(Colors.Grey.Medium);
                    });

                    if (_branding.ShowPageNumbers)
                    {
                        row.ConstantItem(60).AlignRight().Text(text =>
                        {
                            text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                            text.Span(" / ").FontSize(8).FontColor(Colors.Grey.Medium);
                            text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    }
                });
            });
        }
    }
}
