using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>
    /// EP-18 F1: renders a document's formatted reference number by applying
    /// CompanyBranding.DocumentReferenceFormat token substitution ({ORG}/{DOCTYPE}/{YEAR}/{SEQ}).
    /// Known limitation: {SEQ} is the calling entity's own DB id - no dedicated counter table
    /// exists anywhere in this codebase yet, so this is not gap-free sequential numbering.
    /// Building a real DocumentReferenceCounter table is deferred to whichever future feature
    /// needs uniqueness guarantees across concurrent generation.
    /// </summary>
    public class ReferenceNumberComponent : IComponent
    {
        private readonly CompanyBranding _branding;
        private readonly string _documentTypeCode;
        private readonly int _year;
        private readonly long _sequenceValue;

        public ReferenceNumberComponent(CompanyBranding branding, string documentTypeCode, int year, long sequenceValue)
        {
            _branding = branding;
            _documentTypeCode = documentTypeCode;
            _year = year;
            _sequenceValue = sequenceValue;
        }

        public void Compose(IContainer container)
        {
            var referenceNumber = BuildReferenceNumber(_branding, _documentTypeCode, _year, _sequenceValue);
            container.Text(referenceNumber).FontSize(9).FontColor(Colors.Grey.Darken2);
        }

        /// <summary>Lets callers (e.g. DocumentHeaderComponent, or a service persisting the
        /// reference alongside a generated document row) get the plain string without
        /// instantiating this component.</summary>
        public static string BuildReferenceNumber(CompanyBranding branding, string documentTypeCode, int year, long sequenceValue)
        {
            var orgToken = string.IsNullOrWhiteSpace(branding.CompanyName)
                ? "ORG"
                : new string(branding.CompanyName.Where(char.IsUpper).ToArray());

            if (string.IsNullOrWhiteSpace(orgToken))
                orgToken = "ORG";

            return branding.DocumentReferenceFormat
                .Replace("{ORG}", orgToken)
                .Replace("{DOCTYPE}", documentTypeCode)
                .Replace("{YEAR}", year.ToString())
                .Replace("{SEQ}", sequenceValue.ToString());
        }
    }
}
