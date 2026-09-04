using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>EP-18 F1: shared document header - logo, company name, reference number, issue
    /// date, and a divider, arranged per CompanyBranding.HeaderLayout. Logo loading follows the
    /// same try/catch-then-null convention as QuestPdfAdmitCardGenerator.TryLoadPhoto.</summary>
    public class DocumentHeaderComponent : IComponent
    {
        private readonly CompanyBranding _branding;
        private readonly string _documentTitle;
        private readonly string _referenceNumber;
        private readonly DateTime _issueDate;
        private readonly byte[]? _logoBytes;

        public DocumentHeaderComponent(CompanyBranding branding, string documentTitle, string referenceNumber, DateTime issueDate, byte[]? logoBytes = null)
        {
            _branding = branding;
            _documentTitle = documentTitle;
            _referenceNumber = referenceNumber;
            _issueDate = issueDate;
            _logoBytes = logoBytes;
        }

        public void Compose(IContainer container)
        {
            var primaryColor = DocumentFrame.ResolveColor(_branding.PrimaryColor, Colors.Brown.Darken2);
            var accentColor = DocumentFrame.ResolveColor(_branding.AccentColor, Colors.Brown.Lighten4);

            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    if (_branding.HeaderLayout != HeaderLayoutEnum.TextOnly && _logoBytes != null)
                    {
                        row.ConstantItem(56).Height(46).Image(_logoBytes).FitArea();
                    }

                    row.RelativeItem().Column(textColumn =>
                    {
                        textColumn.Item().Text(_documentTitle).FontSize(16).Bold().FontColor(primaryColor);

                        if (!string.IsNullOrWhiteSpace(_branding.CompanyName))
                        {
                            textColumn.Item().Text(_branding.CompanyName).FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });

                    row.ConstantItem(145).AlignRight().Column(metaColumn =>
                    {
                        metaColumn.Item().Text($"Ref: {_referenceNumber}").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                        metaColumn.Item().Text(_issueDate.ToString("dd MMM yyyy")).FontSize(8).FontColor(Colors.Grey.Darken2);
                    });
                });

                if (_branding.HeaderDividerStyle != DocumentDividerStyleEnum.None)
                {
                    column.Item().PaddingTop(5).LineHorizontal(1.2f).LineColor(accentColor);
                }
            });
        }
    }
}
