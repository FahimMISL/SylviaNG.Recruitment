using System.Text;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace SylviaNG.Recruitment.Infrastructure.Services
{
    /// <summary>
    /// Raw text extraction from an uploaded resume file (PDF/DOCX), shared by
    /// HeuristicResumeParsingService and AiResumeParsingService so both parse the same text.
    /// </summary>
    internal static class ResumeTextExtractor
    {
        public static async Task<string> ExtractAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => await ExtractPdfTextAsync(file),
                ".docx" => await ExtractDocxTextAsync(file),
                // The FluentValidation validator already restricts uploads to .pdf/.docx before
                // this runs, so this branch should be unreachable via the API.
                _ => throw new ArgumentException($"Unsupported resume file type: {extension}. Use .pdf or .docx.")
            };
        }

        private static async Task<string> ExtractPdfTextAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var document = PdfDocument.Open(stream);
            var sb = new StringBuilder();
            foreach (var page in document.GetPages())
                sb.AppendLine(page.Text);
            return sb.ToString();
        }

        private static async Task<string> ExtractDocxTextAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var wordDocument = WordprocessingDocument.Open(stream, false);
            return wordDocument.MainDocumentPart?.Document?.Body?.InnerText ?? string.Empty;
        }
    }
}
