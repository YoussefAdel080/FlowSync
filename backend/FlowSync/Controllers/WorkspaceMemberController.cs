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
        public async Task<IActionResult> GetWorkspaceMembers([FromRoute] Guid WorkspaceId,[FromQuery] GetWorkspaceMembersRequest command, CancellationToken token)
        {
            var members = await _workspaceMemberService.GetWorkspaceMembersAsync(WorkspaceId ,command, token);

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
    }
}
