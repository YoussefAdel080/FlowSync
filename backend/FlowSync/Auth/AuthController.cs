using FlowSync.Application.Models;
using FlowSync.Application.Services;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;
using FlowSync.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace FlowSync.Controllers
{
    [ApiController]
    public class Auth : ControllerBase
    {
        private IAuthService _authService;

        public Auth(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost($"{ApiEndpoints.Auth.Register}")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest command, CancellationToken token)
        {
            var user = command.MapToUser();

            var result = await _authService.Register(user, token);

            var response = new BaseResponse<User>
            {
                Success = true,
                Message = "User Registered Successfully.",
                Data = user
            };
            return Ok(response);
        }

        [HttpPost($"{ApiEndpoints.Auth.Login}")]
        public async Task<IActionResult> Login([FromBody] LoginRequest command, CancellationToken token)
        {
            var result = await _authService.Login(command, token);

            return Ok(
                new LoginResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = result
                }
            );
        }

        [HttpPost($"{ApiEndpoints.Auth.Refresh}")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest command, CancellationToken token)
        {
            var result = await _authService.Refresh(command, token);

            return Ok(
                new LoginResponse
                {
                    Success = true,
                    Message = "Refresh successful.",
                    Data = result
                }
            );
        }

        [HttpPost($"{ApiEndpoints.Auth.Logout}")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest command, CancellationToken token)
        {
            var result = await _authService.Logout(command, token);
            return Ok(
                new BaseResponse<bool>
                {
                    Success = true,
                    Message = "Logout successful.",
                    Data = true
                }
            );
        }
    }
}
