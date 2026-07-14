using FlowSync.Application.Configuration;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Security.Cryptography;

namespace FlowSync.Application.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly SmtpOptions _smtpOptions;

        public PasswordResetService(
            IPasswordResetRepository passwordResetRepository,
            IAuthRepository authRepository,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher,
            IOptions<SmtpOptions> options)
        {
            _passwordResetRepository = passwordResetRepository;
            _authRepository = authRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _smtpOptions = options.Value;
        }

        private static string GenerateResetOtp()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, CancellationToken token)
        {
            var user = await _authRepository.GetUserByEmailAsync(email, token);

            // Do not reveal whether the email exists.
            if (user is null)
            {
                return true;
            }

            var otp = GenerateResetOtp();

            await _passwordResetRepository.InvalidateActiveResetsAsync(user.Id, token);
            var stored = await _passwordResetRepository.AddPasswordResetAsync(user.Id, otp, token);

            if (!stored)
            {
                return false;
            }

            await SendEmail(email, otp, token);
            return true;
        }

        private async Task SendEmail(string email, string otp, CancellationToken token)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _smtpOptions.FromName,
                _smtpOptions.FromEmail));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = "Reset your FlowSync Password";

            message.Body = new TextPart("plain")
            {
                Text = $"You requested a password reset for your FlowSync account.\n\nYour reset code is: {otp}\n\nThis code will expire in 10 minutes.\n\nIf you did not request this, you can ignore this email."
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

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken token)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email, token);

            if (user is null)
            {
                return false;
            }

            var hashedPassword = _passwordHasher.HashPassword(user, request.NewPassword);

            var resetSucceeded = await _passwordResetRepository.ResetPasswordAsync(
                request.Email,
                request.Otp,
                hashedPassword,
                token);

            if (!resetSucceeded)
            {
                return false;
            }

            await _tokenService.RevokeAllRefreshTokensForUserAsync(user.Id, token);

            return true;
        }
    }
}
