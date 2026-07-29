namespace SylviaNG.Recruitment.Application.Common.Exceptions
{
    /// <summary>Thrown for any OTP-verification failure - missing/expired challenge, wrong code,
    /// or locked-out. Deliberately one generic exception type (not distinct per case) so the
    /// caller can't distinguish reasons from the shape of the error - the message text is generic,
    /// avoiding an enumeration hint to a brute-forcing caller.</summary>
    public class OtpVerificationException : Exception
    {
        public OtpVerificationException(string message) : base(message)
        {
        }

        public OtpVerificationException()
            : base("Incorrect or expired code.")
        {
        }
    }
}
