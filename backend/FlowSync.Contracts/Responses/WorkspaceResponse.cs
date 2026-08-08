namespace FlowSync.Contracts.Responses
{
    public class WorkspaceResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public WorkspaceOwnerResponse Owner { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class WorkspaceOwnerResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
