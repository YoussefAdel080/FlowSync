using FlowSync.Application.Exceptions;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FlowSync.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IValidator<User> _validator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private readonly IValidator<RefreshRequest> _refreshValidator;
        private readonly IValidator<LogoutRequest> _logoutValidator;
        private readonly IValidator<UpdateProfileRequest> _updateProfileValidator;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly IValidator<VerifyEmailRequest> _verifyEmailValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
        private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;

            public AuthService(IAuthRepository authRepository, IValidator<User> validator, IPasswordHasher<User> passwordHasher, IValidator<LoginRequest> loginValidator, ITokenService tokenService, IValidator<RefreshRequest> refreshValidator, IValidator<LogoutRequest> logoutValidator, IValidator<UpdateProfileRequest> updateProfileValidator, IEmailVerificationService emailVerificationService, IPasswordResetService passwordResetService, IValidator<VerifyEmailRequest> verifyEmailValidator, IValidator<ChangePasswordRequest> changePasswordValidator, IValidator<ForgotPasswordRequest> forgotPasswordValidator, IValidator<ResetPasswordRequest> resetPasswordValidator)
            {
            _authRepository = authRepository;
            _validator = validator;
            _passwordHasher = passwordHasher;
            _loginValidator = loginValidator;
            _tokenService = tokenService;
            _refreshValidator = refreshValidator;
            _logoutValidator = logoutValidator;
            _updateProfileValidator = updateProfileValidator;
            _emailVerificationService = emailVerificationService;
            _passwordResetService = passwordResetService;
            _verifyEmailValidator = verifyEmailValidator;
            _changePasswordValidator = changePasswordValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }

        public async Task<bool> Register(User user, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(user);

            user.Password = _passwordHasher.HashPassword(user, user.Password);

            var result = await _authRepository.Register(user, token);

            await _emailVerificationService.SendVerificationEmailAsync(user.Email, user.Id, token);

            return result;
        }
        public async Task<LoginResponseData> Login(LoginRequest request, CancellationToken token)
        {
            await _loginValidator.ValidateAndThrowAsync(request);

            var user = await _authRepository.GetUserByEmailAsync(request.Email, token);

            if (user is null)
            {
                throw new NotFoundException("User Doesn't Exist");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid Password");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            (string Token, DateTime Expiration)? refreshToken = null;

            if (request.RememberMe)
            {
                refreshToken = _tokenService.GenerateRefreshToken();

                // save refreshToken
                await _tokenService.SaveRefreshTokenAsync(
                    user.Id,
                    refreshToken?.Token!,
                    refreshToken?.Expiration ?? DateTime.UtcNow,
                    token);
            }

            //return new accessToken and refreshToken
            return new LoginResponseData
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiry = accessToken.Expiration,
                RefreshToken = refreshToken?.Token,
                RefreshTokenExpiry = refreshToken?.Expiration,
            };
        }

        public async Task<LoginResponseData?> Refresh(RefreshRequest request, CancellationToken token)
        {
            await _refreshValidator.ValidateAndThrowAsync(request);

            return await _tokenService.RefreshTokenAsync(request.RefreshToken, token);
        }

        public async Task<bool> Logout(LogoutRequest request, CancellationToken cancellationToken)
        {
            await _logoutValidator.ValidateAndThrowAsync(request);

            return await _tokenService.LogoutAsync(request.RefreshToken, cancellationToken);
        }

        public async Task<GetProfileResponse?> GetProfile(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _authRepository.GetUserByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                return null;
            }

            return new GetProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
        }

        public async Task<bool> UpdateProfile(Guid userId,UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _authRepository.GetUserByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                return false;
            }
            await _updateProfileValidator.ValidateAndThrowAsync(request);

            return await _authRepository.UpdateUserProfileAsync(user ,request, cancellationToken);
        }

        public async Task<bool> VerifyEmail(VerifyEmailRequest request, CancellationToken token)
        {
            await _verifyEmailValidator.ValidateAndThrowAsync(request);
            return await _emailVerificationService.VerifyEmailAsync(request.Email,request.Otp, token);
        }

        public async Task<bool> ChangePassword(Guid userId, ChangePasswordRequest request, CancellationToken token)
        {
            await _changePasswordValidator.ValidateAndThrowAsync(request);

            return await _authRepository.ChangePassword(userId, request, token);
        }

        public async Task<bool> ForgotPassword(ForgotPasswordRequest request, CancellationToken token)
        {
            await _forgotPasswordValidator.ValidateAndThrowAsync(request);

            return await _passwordResetService.SendPasswordResetEmailAsync(request.Email, token);
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest request, CancellationToken token)
        {
            await _resetPasswordValidator.ValidateAndThrowAsync(request);

            return await _passwordResetService.ResetPasswordAsync(request, token);
        }
    }
}
