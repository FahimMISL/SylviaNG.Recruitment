using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>EP-18 F1: renders a signature image (or a blank sign-here line if none is
    /// available) with signee name and title, positioned per CompanyBranding.SignaturePosition.</summary>
    public class SignatureBlockComponent : IComponent
    {
        private readonly CompanyBranding _branding;
        private readonly string _signeeName;
        private readonly string _signeeTitle;
        private readonly byte[]? _signatureImageBytes;

        public SignatureBlockComponent(CompanyBranding branding, string signeeName, string signeeTitle, byte[]? signatureImageBytes = null)
        {
            _branding = branding;
            _signeeName = signeeName;
            _signeeTitle = signeeTitle;
            _signatureImageBytes = signatureImageBytes;
        }

        public void Compose(IContainer container)
        {
            if (_branding.SignaturePosition == DocumentElementPositionEnum.None)
                return;

            DocumentPositioning.Apply(container, _branding.SignaturePosition).Column(column =>
            {
                column.Spacing(2);

                if (_signatureImageBytes != null)
                {
                    column.Item().Height(40).Width(120).Image(_signatureImageBytes).FitArea();
                }
                else
                {
                    column.Item().Width(120).Height(30).BorderBottom(1).BorderColor(Colors.Grey.Darken1);
                }

                column.Item().Text(_signeeName).Bold().FontSize(10);
                column.Item().Text(_signeeTitle).FontSize(9).FontColor(Colors.Grey.Darken2);
            });
        }
    }
}
