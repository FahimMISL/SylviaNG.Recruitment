using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using UglyToad.PdfPig;

namespace SylviaNG.Recruitment.Application.Services;

public class CvParsingService : ICvParsingService
{
    public async Task<CvParsedData> ParseCvAsync(Stream fileStream, string fileName)
    {
        var text = await ExtractTextAsync(fileStream, fileName);
        return ParseText(text);
    }

    private async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        ms.Position = 0;

        return ext switch
        {
            ".pdf" => ExtractPdfText(ms),
            ".docx" => ExtractDocxText(ms),
            ".doc" => ExtractLegacyDocText(ms),
            _ => await ReadPlainText(ms),
        };
    }

    private static string ExtractPdfText(MemoryStream ms)
    {
        var sb = new StringBuilder();
        using var document = PdfDocument.Open(ms.ToArray());
        foreach (var page in document.GetPages())
        {
            sb.AppendLine(page.Text);
        }
        return sb.ToString();
    }

    private static string ExtractDocxText(MemoryStream ms)
    {
        var sb = new StringBuilder();
        using var doc = WordprocessingDocument.Open(ms, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body != null)
        {
            sb.Append(body.InnerText);
            foreach (var para in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                sb.AppendLine(para.InnerText);
            }
        }
        return sb.ToString();
    }

    private static string ExtractLegacyDocText(MemoryStream ms)
    {
        var bytes = ms.ToArray();
        var raw = Encoding.UTF8.GetString(bytes);
        var printable = Regex.Replace(raw, @"[^\x20-\x7E\r\n]", " ");
        return Regex.Replace(printable, @"\s{3,}", "\n");
    }

    private static async Task<string> ReadPlainText(MemoryStream ms)
    {
        ms.Position = 0;
        using var reader = new StreamReader(ms);
        return await reader.ReadToEndAsync();
    }

    private CvParsedData ParseText(string text)
    {
        var result = new CvParsedData();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => l.Length > 1)
            .ToList();

        result.Email = ExtractPattern(text, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
        result.Phone = ExtractPattern(text, @"(?:\+?880|01)[0-9\s\-]{8,13}") ?? ExtractPattern(text, @"\+?\d[\d\s\-]{9,14}");
        result.LinkedInUrl = ExtractPattern(text, @"(?:https?://)?(?:www\.)?linkedin\.com/in/[a-zA-Z0-9_-]+/?");
        result.GitHubUrl = ExtractPattern(text, @"(?:https?://)?(?:www\.)?github\.com/[a-zA-Z0-9_-]+/?");
        result.PortfolioUrl = ExtractPattern(text, @"(?:https?://)?(?:www\.)?[a-zA-Z0-9-]+\.[a-zA-Z]{2,}(?:/[^\s]*)?",
            exclude: new[] { "linkedin.com", "github.com", "gmail.com", "yahoo.com", "hotmail.com", "outlook.com" });

        if (lines.Count > 0 && !lines[0].Contains('@') && lines[0].Length < 60)
            result.FullName = lines[0];

        result.Skills = ExtractSkills(text);

        var fieldsFound = 0;
        if (result.Email != null) fieldsFound++;
        if (result.Phone != null) fieldsFound++;
        if (result.FullName != null) fieldsFound++;
        if (result.LinkedInUrl != null) fieldsFound++;
        if (result.Skills.Count > 0) fieldsFound++;
        result.ConfidenceScore = Math.Min(1.0m, fieldsFound * 0.2m);

        return result;
    }

    private string? ExtractPattern(string text, string pattern, string[]? exclude = null)
    {
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        if (!match.Success) return null;
        var value = match.Value.Trim();
        if (exclude != null && exclude.Any(e => value.Contains(e, StringComparison.OrdinalIgnoreCase)))
            return null;
        return value;
    }

    private List<string> ExtractSkills(string text)
    {
        var knownSkills = new[]
        {
            "C#", ".NET", "ASP.NET", "Angular", "React", "JavaScript", "TypeScript", "Python", "Java",
            "SQL", "PostgreSQL", "MySQL", "MongoDB", "Docker", "Kubernetes", "Azure", "AWS", "Git",
            "HTML", "CSS", "REST API", "Microservices", "Entity Framework", "Node.js", "Spring Boot",
            "Machine Learning", "Data Analysis", "Power BI", "Excel", "Tableau",
            "Communication", "Leadership", "Project Management", "Agile", "Scrum",
            "Banking", "Finance", "Accounting", "Risk Management", "Compliance"
        };

        return knownSkills
            .Where(skill => Regex.IsMatch(text, $@"\b{Regex.Escape(skill)}\b", RegexOptions.IgnoreCase))
            .ToList();
    }
}
