namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPlaceholderSubstitutionService
    {
        /// <summary>Replaces every {{Token}} in <paramref name="template"/> with its value from
        /// <paramref name="values"/> (case-insensitive key match). A token with no matching value
        /// is left as-is in the output - deliberately visible rather than silently blanked, so a
        /// typo'd placeholder is obvious in preview instead of vanishing.</summary>
        string Render(string template, IDictionary<string, string> values);

        /// <summary>Returns every distinct {{Token}} name referenced in <paramref name="template"/>,
        /// in first-seen order.</summary>
        List<string> ExtractPlaceholders(string template);
    }
}
