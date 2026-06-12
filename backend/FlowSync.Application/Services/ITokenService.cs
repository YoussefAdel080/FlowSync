using FlowSync.Application.Models;

namespace FlowSync.Application.Services;

public interface ITokenService
{
    (string Token, DateTime Expiration) GenerateAccessToken(User user);

    (string Token, DateTime Expiration) GenerateRefreshToken();
}