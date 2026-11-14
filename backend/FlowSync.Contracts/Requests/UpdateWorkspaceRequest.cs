namespace FlowSync.Contracts.Requests
{
    public class UpdateWorkspaceRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
    }
}
