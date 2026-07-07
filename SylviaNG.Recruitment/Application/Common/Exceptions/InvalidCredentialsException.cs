namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message) : base(message)
        {
        }

        public InvalidCredentialsException()
            : base("Invalid username or password.")
        {
        }
    }
}
