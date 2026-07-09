namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    public class ResourceInUseException : Exception
    {
        public ResourceInUseException(string message) : base(message)
        {
        }

        public ResourceInUseException(string name, object key, int usageCount)
            : base($"Entity \"{name}\" ({key}) is referenced by {usageCount} other record(s) and cannot be deleted.")
        {
        }
    }
}
