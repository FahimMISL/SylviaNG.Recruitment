namespace SylviaNG.Recruitment.SharedKernel.Utils
{
    public static class PersonNameUtility
    {
        public static (string FirstName, string LastName) SplitFullName(string fullName)
        {
            var trimmed = fullName.Trim();
            var spaceIdx = trimmed.IndexOf(' ');
            return spaceIdx < 0
                ? (trimmed, string.Empty)
                : (trimmed[..spaceIdx], trimmed[(spaceIdx + 1)..].Trim());
        }
    }
}
