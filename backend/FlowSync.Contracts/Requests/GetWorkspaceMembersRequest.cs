using FlowSync.Contracts.Enums;

namespace FlowSync.Contracts.Requests
{
    public class GetWorkspaceMembersRequest : PaginationRequest
    {
        public string? MemberName { get; set; } = string.Empty;
        public WorkspaceRole? Role { get; set; }
        public DateTime? JoinedAtFrom { get; set; }
        public DateTime? JoinedAtTo { get; set; }
        public WorkspaceMemberSortBy? SortByValue { get; set; }
    }
}
