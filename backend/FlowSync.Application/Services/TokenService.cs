using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FlowSync.Application.Configuration;
using FlowSync.Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlowSync.Application.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public TokenService(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;
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
}