namespace FlowSync.Application.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; }
        public ICollection<WorkspaceMember> WorkspaceMemberships { get; set; }
        public ICollection<WorkspaceInvitation> WorkspaceInvitations { get; set; }
    }
}
