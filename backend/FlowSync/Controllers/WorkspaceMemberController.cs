using FlowSync.Application.Services;
using FlowSync.Contracts.Requests;
using FlowSync.Contracts.Responses;
using FlowSync.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowSync.Controllers
{
    [ApiController]
    public class WorkspaceMemberController : ControllerBase
    {
        private readonly IWorkspaceMemberService _workspaceMemberService;

        public WorkspaceMemberController(IWorkspaceMemberService workspaceMemberService)
        {
            _workspaceMemberService = workspaceMemberService;
        }

        [Authorize]
        [HttpGet($"{ApiEndpoints.WorkspaceMembers.GetWorkspaceMembers}")]
        public async Task<IActionResult> GetWorkspaceMembers([FromRoute] Guid workspaceId,[FromQuery] GetWorkspaceMembersRequest command, CancellationToken token)
        {
            var members = await _workspaceMemberService.GetWorkspaceMembersAsync(workspaceId ,command, token);

            return Ok(new PaginationResponse<WorkspaceMemberResponse>()
            {
                Success = true,
                Message = "Workspace members fetched successfully",
                Data = members.Items.MapToWorkspaceMembersResponse(),
                PageNumber = members.PageNumber,
                PageSize = members.PageSize,
                TotalCount = members.TotalCount,
                TotalPages = members.TotalPages,
            });
        }

        [Authorize]
        [HttpPut($"{ApiEndpoints.WorkspaceMembers.ChangeRole}")]
        public async Task<IActionResult> ChangeWorkspaceMemberRole([FromRoute] Guid workspaceId, [FromQuery] ChangeWorkspaceMemberRoleRequest command, CancellationToken token)
        {
            var result = await _workspaceMemberService.ChangeWorkspaceMemberRoleAsync(workspaceId ,command, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Role Changed Successfully.",
                Data = result
            });
        }

        [Authorize]
        [HttpDelete($"{ApiEndpoints.WorkspaceMembers.Remove}")]
        public async Task<IActionResult> RemoveWorkspaceMember([FromRoute] Guid workspaceId, [FromQuery] RemoveWorkspaceMemberRequest command, CancellationToken token)
        {
            var result = await _workspaceMemberService.RemoveWorkspaceMemberAsync(workspaceId ,command, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Member Removed Successfully.",
                Data = result
            });
        }

    }
}
