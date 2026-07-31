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
    public class WorkspaceController : ControllerBase
    {
        private readonly IWorkspaceService _workspaceService;

        public WorkspaceController(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [Authorize]
        [HttpGet($"{ApiEndpoints.Workspace.MyWorkspaces}")]
        public async Task<IActionResult> GetMyWorkSpaces(CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceService.GetMyWorkspacesAsync(userId.Value, token);

            return Ok(new BaseResponse<IEnumerable<WorkspaceResponse>>
            {
                Success = true,
                Message = "Workspace Fetched Successfully.",
                Data = result.MapToWorkspaceResponse()
            });
        }

        [Authorize]
        [HttpGet($"{ApiEndpoints.Workspace.GetById}")]
        public async Task<IActionResult> GetWorkspaceById([FromRoute] Guid id, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceService.GetWorkspaceByIdAsync(id, userId.Value, token);

            return Ok(new BaseResponse<WorkspaceResponse>
            {
                Success = true,
                Message = "Workspace Fetched Successfully.",
                Data = result?.MapToWorkspaceResponse()
            });
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

        [Authorize]
        [HttpDelete($"{ApiEndpoints.Workspace.Delete}")]
        public async Task<IActionResult> DeleteWorkSpace([FromBody] DeleteWorkspaceRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceService.DeleteWorkspaceAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Deleted Successfully.",
                Data = true
            });
        }
    }
}
