using System.ComponentModel.DataAnnotations.Schema;

namespace FlowSync.Application.Models
{
    public class Workspace
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
