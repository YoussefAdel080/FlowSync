
using FlowSync.Application.Configuration;
using FlowSync.Application.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Security.Cryptography;

namespace FlowSync.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly SmtpOptions _smtpOptions;

        public EmailVerificationService(IEmailVerificationRepository emailVerificationRepository, IOptions<SmtpOptions> options)
        {
            _emailVerificationRepository = emailVerificationRepository;
            _smtpOptions = options.Value;
        }

        private string GenerateVerificationOtp()
        {
            var otp = RandomNumberGenerator.GetInt32(
                100000,
                1000000)
                .ToString();
            return otp;
        }

        public async Task<bool> SendVerificationEmailAsync(string email, Guid userId, CancellationToken token)
        {
            var otp = GenerateVerificationOtp();

            await SendEmail(email, otp, token);

            await _emailVerificationRepository.InvalidateActiveVerificationsAsync(userId, token);
            var result = await _emailVerificationRepository.AddEmailVerificationAsync(userId, otp, token);

            return true;
        }

        private async Task SendEmail(string email, string otp, CancellationToken token)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _smtpOptions.FromName,
                _smtpOptions.FromEmail));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Verify your FlowSync Account";

            message.Body = new TextPart("plain")
            {
                Text = $"Welcome to FlowSync!\n\nYour verification code is: {otp}\n\nThis code will expire in 10 minutes."
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _smtpOptions.Host,
                _smtpOptions.Port,
                SecureSocketOptions.StartTls,
                token);

            await smtp.AuthenticateAsync(
                _smtpOptions.Username,
                _smtpOptions.Password,
                token);

            await smtp.SendAsync(message, token);

            await smtp.DisconnectAsync(true, token);
        }

        public async Task<bool> VerifyEmailAsync(string email ,string otp, CancellationToken token)
        {
            return await _emailVerificationRepository.VerifyEmailAsync(email, otp, token);
        }
    }
}
