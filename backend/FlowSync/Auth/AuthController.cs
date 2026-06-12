using FlowSync.Application.Models;
using FlowSync.Application.Services;
using FlowSync.Contracts.Requests;
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
                Message = "User Registered Successfully",
                Data = user
            };
            return Ok(response);
        }
    }
}
