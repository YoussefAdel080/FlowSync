using FlowSync.Application.Models;
using FlowSync.Application.Services;
using FlowSync.Auth;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;
using FlowSync.Mapping;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet($"{ApiEndpoints.Auth.Profile}")]
        public async Task<IActionResult> GetProfile(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }
            var result = await _authService.GetProfile(userId.Value, token);
            if (result == null)
            {
                return NotFound(new BaseResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });
            }
            return Ok(new BaseResponse<GetProfileResponse> {
                Success = true,
                Message = "Profile retrieved successfully.",
                Data = result
            });
        }

        [Authorize]
        [HttpPut($"{ApiEndpoints.Auth.Profile}")]
        public async Task<IActionResult> PutProfile([FromBody] UpdateProfileRequest request, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if(userId is null)
            {
                return Unauthorized();
            }
            var result = await _authService.UpdateProfile(userId.Value, request, token);
            if (!result)
            {
                return NotFound(new BaseResponse<object>
                {
                    Success = false,
                    Message = "User not found.",
                    Data = null
                });
            }
            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Profile updated successfully.",
                Data = true
            });
        }
    }
}
