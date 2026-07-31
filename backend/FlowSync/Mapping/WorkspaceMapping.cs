using FlowSync.Application.Models;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Responses;

namespace FlowSync.Mapping
{
    public static class WorkspaceMapping
    {
        public static WorkspaceResponse MapToWorkspaceResponse(this Workspace workspace)
        {
            var ownerMember = workspace.Members
                .FirstOrDefault(m => m.Role == WorkspaceRole.Owner);

            return new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                OwnerId = ownerMember.UserId,
                CreatedAt = workspace.CreatedAt,
                Owner = new WorkspaceOwnerResponse
                {
                    Id = ownerMember.User.Id,
                    FirstName = ownerMember.User.FirstName,
                    LastName = ownerMember.User.LastName,
                    Email = ownerMember.User.Email
                }
            };
        }

        public static IEnumerable<WorkspaceResponse> MapToWorkspaceResponse(
            this IEnumerable<Workspace> workspaces)
        {
            return workspaces.Select(workspace => workspace.MapToWorkspaceResponse());
        }
    }
}
