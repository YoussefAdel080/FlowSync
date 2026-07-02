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

        public AuthService(IAuthRepository authRepository, IValidator<User> validator, IPasswordHasher<User> passwordHasher, IValidator<LoginRequest> loginValidator, ITokenService tokenService, IValidator<RefreshRequest> refreshValidator)
        {
            _authRepository = authRepository;
            _validator = validator;
            _passwordHasher = passwordHasher;
            _loginValidator = loginValidator;
            _tokenService = tokenService;
            _refreshValidator = refreshValidator;
        }

        public async Task<bool> Register(User user, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(user);

            user.Password = _passwordHasher.HashPassword(user, user.Password);

            return await _authRepository.Register(user, token);
        }
        public async Task<LoginResponseData> Login(LoginRequest request, CancellationToken token)
        {
            await _loginValidator.ValidateAndThrowAsync(request);

            var user = await _authRepository.GetUserByEmailAsync(request.Email, token);

            if (user is null)
            {
                throw new Exception("User Doesn't Exist");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.Password,
                request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid Password");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            (string Token, DateTime Expiration)? refreshToken = null;

            if (request.RememberMe){ 
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
    }
}
