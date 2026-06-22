using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class EmailOtpService : IEmailOtpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private const int OtpExpiryMinutes = 5;
        private const int MaxRequestsPerHour = 3;

        public EmailOtpService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Message)> SendOtpAsync(string email)
        {
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            var recentCount = await _unitOfWork.Context.Set<EmailOtp>()
                .CountAsync(o => o.Email == email && o.CreatedAt >= oneHourAgo);

            if (recentCount >= MaxRequestsPerHour)
                return (false, "Too many OTP requests. Please try again later.");

            var otpCode = GenerateOtp();
            var otp = new EmailOtp
            {
                Email = email,
                OtpCode = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes),
                IsUsed = false,
                AttemptCount = 0,
                IsActive = true
            };

            await _unitOfWork.Context.Set<EmailOtp>().AddAsync(otp);
            await _unitOfWork.SaveChangesAsync();

            var emailBody = EmailTemplates.EmailOtpVerification(otpCode, OtpExpiryMinutes);
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendAsync(email, email, "Email Verification Code", emailBody);
                }
                catch { }
            });

            return (true, "OTP sent to your email.");
        }

        public async Task<(bool Success, string Message)> VerifyOtpAsync(string email, string otpCode)
        {
            var otp = await _unitOfWork.Context.Set<EmailOtp>()
                .Where(o => o.Email == email && !o.IsUsed && o.IsActive)
                .OrderByDescending(o => o.ExpiresAt)
                .FirstOrDefaultAsync();

            if (otp == null)
                return (false, "No OTP found. Please request a new one.");

            if (otp.ExpiresAt < DateTime.UtcNow)
                return (false, "OTP has expired. Please request a new one.");

            otp.AttemptCount++;
            if (otp.AttemptCount > 5)
            {
                otp.IsUsed = true;
                await _unitOfWork.SaveChangesAsync();
                return (false, "Too many attempts. Please request a new OTP.");
            }

            if (otp.OtpCode != otpCode)
            {
                await _unitOfWork.SaveChangesAsync();
                return (false, "Invalid OTP code.");
            }

            otp.IsUsed = true;
            await _unitOfWork.SaveChangesAsync();
            return (true, "Email verified successfully.");
        }

        private static string GenerateOtp()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }
    }
}
