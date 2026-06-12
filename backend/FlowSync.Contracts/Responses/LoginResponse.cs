using FlowSync.Contracts.Requests;

namespace FlowSync.Contracts.Responses
{
    public class LoginResponse: BaseResponse<LoginResponseData>
    {
    }

    public class LoginResponseData
    {
        public string AccessToken { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime AccessTokenExpiry { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
