using FlowSync.Application.Services;
using FlowSync.Auth;
using FlowSync.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowSync.Controllers
{
    [ApiController]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspacesController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [Authorize]
        [HttpPost($"{ApiEndpoints.Workspace.Create}")]
        public async Task<IActionResult> CreateWorkSpace([FromBody] CreateWorkspaceRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceService.CreateWorkspaceAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Created Successfully.",
                Data = true
            });
        }

        [Authorize]
        [HttpPut($"{ApiEndpoints.Workspace.Update}")]
        public async Task<IActionResult> UpdateWorkSpace([FromBody] UpdateWorkspaceRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceService.UpdateWorkspaceAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Updated Successfully.",
                Data = true
            });
        }
    }
}
