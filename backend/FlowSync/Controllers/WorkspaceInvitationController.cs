using FlowSync.Application.Services;
using FlowSync.Auth;
using FlowSync.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowSync.Controllers
{
    [ApiController]
    public class WorkspaceInvitationController : ControllerBase
    {
        private readonly IWorkspaceInvitationService _workspaceInvitationService;

        public WorkspaceInvitationController(IWorkspaceInvitationService workspaceInvitationService)
        {
            _workspaceInvitationService = workspaceInvitationService;
        }

        [Authorize]
        [HttpPost($"{ApiEndpoints.WorkspaceInvitation.Invite}")]
        public async Task<IActionResult> InviteToWorkspace([FromBody] InviteToWorkspaceRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceInvitationService.CreateWorkspaceInvitationAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Invitation Created Successfully.",
                Data = true
            });
        }
        [Authorize]
        [HttpPut($"{ApiEndpoints.WorkspaceInvitation.Accept}")]
        public async Task<IActionResult> AcceptWorkspaceInvitation([FromBody] AcceptWorkspaceInvitationRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceInvitationService.AcceptWorkspaceInvitationAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Invitation Accepted Successfully.",
                Data = true
            });
        }

        [Authorize]
        [HttpPut($"{ApiEndpoints.WorkspaceInvitation.Decline}")]
        public async Task<IActionResult> DeclineWorkspaceInvitation([FromBody] DeclineWorkspaceInvitationRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceInvitationService.DeclineWorkspaceInvitationAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Invitation Declined Successfully.",
                Data = true
            });
        }

        [Authorize]
        [HttpPut($"{ApiEndpoints.WorkspaceInvitation.Cancel}")]
        public async Task<IActionResult> CancelWorkspaceInvitation([FromBody] CancelWorkspaceInvitationRequest command, CancellationToken token)
        {
            var userId = HttpContext.GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _workspaceInvitationService.CancelWorkspaceInvitationAsync(command, userId.Value, token);

            return Ok(new BaseResponse<bool>
            {
                Success = true,
                Message = "Workspace Invitation Canceled Successfully.",
                Data = true
            });
        }
    }
}
