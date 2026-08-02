using FlowSync.Application.Models;
using FlowSync.Contracts.Responses;

namespace FlowSync.Mapping
{
    public static class WorkspaceMemberMapping
    {
        public static WorkspaceMemberResponse MapToWorkspaceMemberResponse(this WorkspaceMember member)
        {
            return new WorkspaceMemberResponse()
            {
                Id = member.Id,
                UserId = member.UserId,
                DisplayName = member.User.DisplayName,
                Email = member.User.Email,
                Role = member.Role,
                JoinedAt = member.JoinedAt,
                InvitationId = member.WorkspaceInvitationId,
                InvitedBy = new InvitedByResponse()
                {
                    Id = member?.WorkspaceInvitation?.InvitedById,
                    DisplayName = member?.WorkspaceInvitation?.InvitedBy.DisplayName,
                    Role = member?.WorkspaceInvitation?.Role,
                    Email = member?.WorkspaceInvitation?.InvitedBy.Email,
                },
            };
        }

        public static List<WorkspaceMemberResponse> MapToWorkspaceMembersResponse(this List<WorkspaceMember> members)
        {
            return members.Select(member => member.MapToWorkspaceMemberResponse()).ToList();
        }
    }
}
