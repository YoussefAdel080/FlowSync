using FlowSync.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowSync.Application.Contexts
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
