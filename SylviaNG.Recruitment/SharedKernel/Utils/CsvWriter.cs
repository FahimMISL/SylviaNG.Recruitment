using System.Text;

namespace SylviaNG.Recruitment.SharedKernel.Utils
{
    /// <summary>
    /// Shared CSV-building helper, extracted from ExportGenerationService (EP-13 US-100) so
    /// EP-14 US-106/107's analytics CSV exports don't duplicate the same escape logic.
    /// </summary>
    public static class CsvWriter
    {
        public static byte[] Write(string[] headers, List<string[]> rows)
        {
            var builder = new StringBuilder();
            builder.AppendLine(string.Join(",", headers.Select(EscapeField)));

            foreach (var row in rows)
                builder.AppendLine(string.Join(",", row.Select(EscapeField)));

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        public static string EscapeField(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }
    }
}
