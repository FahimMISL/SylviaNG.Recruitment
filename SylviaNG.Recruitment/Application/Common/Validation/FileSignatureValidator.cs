using Microsoft.AspNetCore.Http;

namespace SylviaNG.Recruitment.Application.Common.Validation
{
    /// <summary>
    /// Confirms an uploaded file's actual content matches its extension via magic-byte
    /// signatures, instead of trusting the extension (or client-supplied Content-Type) alone -
    /// every upload validator in the app previously only checked Path.GetExtension(f.FileName)
    /// against an allow-list, so a renamed polyglot (e.g. an HTML/script payload saved as
    /// "resume.docx") was accepted outright.
    /// </summary>
    public static class FileSignatureValidator
    {
        // .doc/.xls (legacy OLE compound file) share one signature; .docx/.xlsx (OOXML) are zip
        // containers and share the zip signature - this can't distinguish docx from xlsx by magic
        // bytes alone, but it does reject anything that isn't a real container of that family.
        private static readonly Dictionary<string, byte[][]> SignaturesByExtension = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = new[] { new byte[] { 0x25, 0x50, 0x44, 0x46 } },
            [".png"] = new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            [".jpg"] = new[] { new byte[] { 0xFF, 0xD8, 0xFF } },
            [".jpeg"] = new[] { new byte[] { 0xFF, 0xD8, 0xFF } },
            [".doc"] = new[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
            [".xls"] = new[] { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } },
            [".docx"] = new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
            [".xlsx"] = new[] { new byte[] { 0x50, 0x4B, 0x03, 0x04 } },
        };

        /// <summary>
        /// True if the extension has no known signature (e.g. .csv is plain text with nothing to
        /// sniff) or if the file's leading bytes match one of that extension's known signatures.
        /// </summary>
        public static bool MatchesExtension(IFormFile file, string extension)
        {
            if (!SignaturesByExtension.TryGetValue(extension, out var signatures))
                return true;

            var maxLength = signatures.Max(s => s.Length);
            var header = new byte[maxLength];

            using var stream = file.OpenReadStream();
            var bytesRead = stream.Read(header, 0, maxLength);
            stream.Seek(0, SeekOrigin.Begin);

            if (bytesRead < maxLength)
                return false;

            return signatures.Any(signature => header.AsSpan(0, signature.Length).SequenceEqual(signature));
        }
    }
}
