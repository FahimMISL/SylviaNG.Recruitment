namespace SylviaNG.Recruitment.Infrastructure.Services;

public static class EmailTemplates
{
    private static string Wrap(string title, string bodyContent) => $"""
        <!DOCTYPE html>
        <html>
        <head><meta charset="utf-8"/></head>
        <body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;background:#f4f5f7;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background:#f4f5f7;padding:32px 0;">
            <tr><td align="center">
              <table width="560" cellpadding="0" cellspacing="0" style="background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.06);">
                <!-- Header -->
                <tr>
                  <td style="background:#0a1628;padding:24px 32px;">
                    <h1 style="margin:0;color:#ffffff;font-size:20px;font-weight:700;">Smart Recruitment Platform</h1>
                    <p style="margin:4px 0 0;color:rgba(255,255,255,0.7);font-size:12px;">Powered by Millennium Information Solution Ltd.</p>
                  </td>
                </tr>
                <!-- Body -->
                <tr>
                  <td style="padding:32px;">
                    <h2 style="margin:0 0 16px;color:#1a1a2e;font-size:18px;">{title}</h2>
                    {bodyContent}
                  </td>
                </tr>
                <!-- Footer -->
                <tr>
                  <td style="background:#f8f9fa;padding:16px 32px;text-align:center;">
                    <p style="margin:0;color:#999;font-size:11px;">This is an automated message. Please do not reply directly.</p>
                    <p style="margin:4px 0 0;color:#bbb;font-size:11px;">&copy; 2026 Millennium Information Solution Ltd. All rights reserved.</p>
                  </td>
                </tr>
              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;

    public static string AccountCreated(string fullName, string email, string role) =>
        Wrap("Your Account Has Been Created",
            $"""
            <p style="color:#555;line-height:1.6;">Dear <strong>{fullName}</strong>,</p>
            <p style="color:#555;line-height:1.6;">An account has been created for you on the Smart Recruitment Platform.</p>
            <table style="background:#f8f9fa;border-radius:6px;padding:16px;width:100%;margin:16px 0;" cellpadding="8">
              <tr><td style="color:#888;font-size:13px;width:120px;">Email</td><td style="color:#1a1a2e;font-weight:600;">{email}</td></tr>
              <tr><td style="color:#888;font-size:13px;">Username</td><td style="color:#1a1a2e;font-weight:600;">{email.Split('@')[0]}</td></tr>
              <tr><td style="color:#888;font-size:13px;">Role</td><td style="color:#c8102e;font-weight:600;">{role}</td></tr>
            </table>
            <p style="color:#555;line-height:1.6;">A temporary password has been set for your account. When you log in for the first time, you will be prompted to create a new password.</p>
            <div style="text-align:center;margin:24px 0;">
              <a href="http://localhost:4200" style="display:inline-block;background:#c8102e;color:#ffffff;text-decoration:none;padding:12px 32px;border-radius:6px;font-weight:600;font-size:14px;">Log In to Your Account</a>
            </div>
            <p style="color:#c8102e;font-size:12px;margin-top:16px;"><strong>Security Notice:</strong> If you did not expect this account, please contact your administrator.</p>
            """);

    public static string CandidateWelcome(string fullName, string email) =>
        Wrap("Welcome to Smart Recruitment Platform!",
            $"""
            <p style="color:#555;line-height:1.6;">Dear <strong>{fullName}</strong>,</p>
            <p style="color:#555;line-height:1.6;">Welcome! Your candidate account has been successfully created.</p>
            <table style="background:#f8f9fa;border-radius:6px;padding:16px;width:100%;margin:16px 0;" cellpadding="8">
              <tr><td style="color:#888;font-size:13px;width:120px;">Email</td><td style="color:#1a1a2e;font-weight:600;">{email}</td></tr>
              <tr><td style="color:#888;font-size:13px;">Account Type</td><td style="color:#c8102e;font-weight:600;">Candidate</td></tr>
            </table>
            <p style="color:#555;line-height:1.6;">To improve your chances, complete your profile to at least <strong>70%</strong> before applying for jobs.</p>
            <p style="color:#555;line-height:1.6;">Good luck with your job search!</p>
            """);

    public static string ApplicationSubmittedCandidate(string candidateName, string jobTitle) =>
        Wrap("Application Submitted Successfully",
            $"""
            <p style="color:#555;line-height:1.6;">Dear <strong>{candidateName}</strong>,</p>
            <p style="color:#555;line-height:1.6;">Your application for the following position has been submitted successfully:</p>
            <div style="background:#f8f9fa;border-left:4px solid #c8102e;border-radius:4px;padding:16px;margin:16px 0;">
              <p style="margin:0;color:#1a1a2e;font-size:16px;font-weight:700;">{jobTitle}</p>
              <p style="margin:4px 0 0;color:#888;font-size:13px;">Millennium Information Solution Ltd.</p>
            </div>
            <p style="color:#555;line-height:1.6;">Our recruitment team will review your application and get back to you. You can track the status from your dashboard.</p>
            """);

    public static string ApplicationStatusUpdate(string candidateName, string jobTitle, string newStatus) =>
        Wrap("Application Status Updated",
            $"""
            <p style="color:#555;line-height:1.6;">Dear <strong>{candidateName}</strong>,</p>
            <p style="color:#555;line-height:1.6;">There has been an update on your application:</p>
            <div style="background:#f8f9fa;border-left:4px solid #c8102e;border-radius:4px;padding:16px;margin:16px 0;">
              <p style="margin:0;color:#1a1a2e;font-size:16px;font-weight:700;">{jobTitle}</p>
              <p style="margin:8px 0 0;color:#555;font-size:14px;">New Status: <strong style="color:#c8102e;">{newStatus}</strong></p>
            </div>
            <p style="color:#555;line-height:1.6;">Log in to your dashboard to view more details and track your application progress.</p>
            <div style="text-align:center;margin:24px 0;">
              <a href="http://localhost:4200/my-applications" style="display:inline-block;background:#c8102e;color:#ffffff;text-decoration:none;padding:12px 32px;border-radius:6px;font-weight:600;font-size:14px;">View My Applications</a>
            </div>
            """);

    public static string InterviewScheduled(string candidateName, string jobTitle, DateTime scheduledDate, string location, string? meetingLink) =>
        Wrap("Interview Scheduled",
            $"""
            <p style="color:#555;line-height:1.6;">Dear <strong>{candidateName}</strong>,</p>
            <p style="color:#555;line-height:1.6;">We are pleased to inform you that an interview has been scheduled for your application:</p>
            <table style="background:#f8f9fa;border-radius:6px;padding:16px;width:100%;margin:16px 0;" cellpadding="8">
              <tr><td style="color:#888;font-size:13px;width:120px;">Position</td><td style="color:#1a1a2e;font-weight:700;">{jobTitle}</td></tr>
              <tr><td style="color:#888;font-size:13px;">Date &amp; Time</td><td style="color:#1a1a2e;font-weight:600;">{scheduledDate:dddd, MMMM dd, yyyy 'at' hh:mm tt} (UTC)</td></tr>
              <tr><td style="color:#888;font-size:13px;">Location</td><td style="color:#1a1a2e;font-weight:600;">{location}</td></tr>
              {(string.IsNullOrEmpty(meetingLink) ? "" : $"""<tr><td style="color:#888;font-size:13px;">Meeting Link</td><td><a href="{meetingLink}" style="color:#c8102e;font-weight:600;text-decoration:underline;">Join Meeting</a></td></tr>""")}
            </table>
            <p style="color:#555;line-height:1.6;">Please ensure you are available at the scheduled time. If you have any concerns, contact us through the platform.</p>
            <div style="text-align:center;margin:24px 0;">
              <a href="http://localhost:4200/my-applications" style="display:inline-block;background:#c8102e;color:#ffffff;text-decoration:none;padding:12px 32px;border-radius:6px;font-weight:600;font-size:14px;">View My Applications</a>
            </div>
            """);

    public static string EmailOtpVerification(string otpCode, int expiryMinutes) =>
        Wrap("Email Verification Code",
            $"""
            <p style="color:#555;line-height:1.6;">Use the following code to verify your email address:</p>
            <div style="text-align:center;margin:24px 0;">
              <div style="display:inline-block;background:#f8f9fa;border:2px solid #c8102e;border-radius:8px;padding:16px 32px;">
                <span style="font-size:32px;font-weight:700;letter-spacing:8px;color:#1a1a2e;">{otpCode}</span>
              </div>
            </div>
            <p style="color:#555;line-height:1.6;">This code will expire in <strong>{expiryMinutes} minutes</strong>.</p>
            <p style="color:#c8102e;font-size:12px;margin-top:16px;"><strong>Security Notice:</strong> Do not share this code with anyone.</p>
            """);

    public static string ApplicationReceivedHR(string candidateName, string candidateEmail, string jobTitle) =>
        Wrap("New Job Application Received",
            $"""
            <p style="color:#555;line-height:1.6;">A new application has been received for review:</p>
            <table style="background:#f8f9fa;border-radius:6px;padding:16px;width:100%;margin:16px 0;" cellpadding="8">
              <tr><td style="color:#888;font-size:13px;width:120px;">Position</td><td style="color:#1a1a2e;font-weight:700;">{jobTitle}</td></tr>
              <tr><td style="color:#888;font-size:13px;">Candidate</td><td style="color:#1a1a2e;font-weight:600;">{candidateName}</td></tr>
              <tr><td style="color:#888;font-size:13px;">Email</td><td style="color:#1a1a2e;">{candidateEmail}</td></tr>
            </table>
            <p style="color:#555;line-height:1.6;">Please log in to the recruitment dashboard to review this application.</p>
            """);
}
