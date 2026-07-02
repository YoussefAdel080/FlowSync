using FlowSync.Application.Configuration;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Contracts.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlowSync.Application.Services;

public class TokenService : ITokenService
{
    private readonly ITokenRepository _refreshTokenRepository;
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> options, ITokenRepository refreshTokenRepository)
    {
        _jwtOptions = options.Value;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public (string Token, DateTime Expiration) GenerateAccessToken(User user)
    {
        var expiration =
            DateTime.UtcNow.AddMinutes(
                _jwtOptions.AccessTokenExpirationMinutes);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new(
                ClaimTypes.Email,
                user.Email),

            new(
                ClaimTypes.Name,
                $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return (accessToken, expiration);
    }

    public (string Token, DateTime Expiration) GenerateRefreshToken()
    {
        var expiration =
            DateTime.UtcNow.AddDays(
                _jwtOptions.RefreshTokenExpirationDays);

        var refreshToken =
            Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));

        return (refreshToken, expiration);
    }

    public async Task<bool> SaveRefreshTokenAsync(Guid userId, string token, DateTime expiresAt, CancellationToken cancellationToken)
    {
          var refreshToken = new RefreshToken
          {
              UserId = userId,
              Token = token,
              ExpiresAt = expiresAt,
              CreatedAt = DateTime.UtcNow
          };

          return await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(token, cancellationToken);
        return refreshToken;
    }

    public async Task<LoginResponseData?> RefreshTokenAsync(string token, CancellationToken cancellationToken)
    {
        var newRefreshTokenValue = GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshTokenValue.Token,
            ExpiresAt = newRefreshTokenValue.Expiration,
            CreatedAt = DateTime.UtcNow
        };

        var oldToken = await _refreshTokenRepository.RotateRefreshTokenAsync(
            token,
            newRefreshTokenEntity,
            cancellationToken);

        if (oldToken?.User is null)
            return null;

        var newAccessToken = GenerateAccessToken(oldToken.User);

        return new LoginResponseData
        {
            AccessToken = newAccessToken.Token,
            AccessTokenExpiry = newAccessToken.Expiration,
            RefreshToken = newRefreshTokenValue.Token,
            RefreshTokenExpiry = newRefreshTokenValue.Expiration
        };
    }
}