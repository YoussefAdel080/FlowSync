using FlowSync.Application.Models;
using FlowSync.Contracts.Responses;

namespace FlowSync.Mapping
{
    public static class WorkspaceMapping
    {
        public static WorkspaceResponse MapToWorkspaceResponse(this Workspace workspace)
        {
            return new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                OwnerId = workspace.OwnerId,
                CreatedAt = workspace.CreatedAt,
                Owner = new WorkspaceOwnerResponse
                {
                    Id = workspace.Owner.Id,
                    FirstName = workspace.Owner.FirstName,
                    LastName = workspace.Owner.LastName,
                    Email = workspace.Owner.Email
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
