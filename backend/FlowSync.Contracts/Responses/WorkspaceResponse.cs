namespace FlowSync.Contracts.Responses
{
    public class WorkspaceResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public WorkspaceOwnerResponse Owner { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class WorkspaceOwnerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
