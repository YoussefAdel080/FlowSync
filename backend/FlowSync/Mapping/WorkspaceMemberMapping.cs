using FlowSync.Application.Models;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Responses;

namespace FlowSync.Mapping
{
    public static class WorkspaceMemberMapping
    {
        public static WorkspaceMemberResponse MapToWorkspaceMemberResponse(this WorkspaceMember member)
        {
            var workspaceMemberResponse = 
                new WorkspaceMemberResponse()
                {
                    Id = member.Id,
                    UserId = member.UserId,
                    DisplayName = member.User.DisplayName,
                    Email = member.User.Email,
                    Role = member.Role,
                    JoinedAt = member.JoinedAt,
                    InvitationId = member.WorkspaceInvitationId
                };

            var invitation = member.WorkspaceInvitation;
            if (invitation != null)
            {
                workspaceMemberResponse.InvitedBy = new InvitedByResponse()
                {
                    Id = invitation.InvitedById,
                    DisplayName = invitation.InvitedBy.DisplayName,
                    Role = (WorkspaceRole)invitation.Role,
                    Email = invitation.InvitedBy.Email
                };
            }

            return workspaceMemberResponse;
        }

        public static List<WorkspaceMemberResponse> MapToWorkspaceMembersResponse(this List<WorkspaceMember> members)
        {
            return members.Select(member => member.MapToWorkspaceMemberResponse()).ToList();
        }
    }
}
