using System.Text.RegularExpressions;

namespace SylviaNG.Recruitment.Application.Common.Utilities
{
    /// <summary>
    /// Normalizes phone numbers for equality comparison (duplicate detection, US-038 AC1).
    /// Strips all non-digit characters, then compares on the trailing digits only, so leading
    /// '0' vs '+880' country-code variants of the same number still match.
    /// </summary>
    public static class PhoneNormalizer
    {
        private const int SignificantDigits = 9;

        public static string? Normalize(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var digits = Regex.Replace(phone, @"\D", "");
            if (digits.Length == 0)
                return null;

            return digits.Length <= SignificantDigits
                ? digits
                : digits[^SignificantDigits..];
        }
    }
}
