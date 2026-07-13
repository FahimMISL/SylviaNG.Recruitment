namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    public class InvalidStatusTransitionException : Exception
    {
        public InvalidStatusTransitionException(string message) : base(message)
        {
        }

        public InvalidStatusTransitionException(string name, object currentStatus, object requestedStatus)
            : base($"Entity \"{name}\" cannot transition from status \"{currentStatus}\" to \"{requestedStatus}\".")
        {
        }
    }
}
