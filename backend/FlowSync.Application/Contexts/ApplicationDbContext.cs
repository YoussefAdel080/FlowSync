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
        public DbSet<EmailVerification> EmailVerifications { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.HasIndex(rt => rt.Token)
                    .IsUnique();

                entity.HasOne(rt => rt.User)
                    .WithMany()
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rt => rt.ReplacedByToken)
                    .WithMany()
                    .HasForeignKey(rt => rt.ReplacedByTokenId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Workspace>(entity =>
            {
                entity.HasKey(w => w.Id);

                entity.HasMany(w => w.Members)
                    .WithOne(wm => wm.Workspace)
                    .HasForeignKey(wm => wm.WorkspaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WorkspaceMember>(entity =>
            {
                entity.HasKey(wm => wm.Id);

                entity.HasIndex(wm => new { wm.WorkspaceId, wm.UserId })
                    .IsUnique();

                entity.Property(wm => wm.Role)
                    .HasConversion<int>();

                entity.Property(wm => wm.JoinedAt)
                    .IsRequired();

                entity.HasOne(wm => wm.User)
                    .WithMany(u => u.WorkspaceMemberships)
                    .HasForeignKey(wm => wm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wm => wm.Workspace)
                    .WithMany(w => w.Members)
                    .HasForeignKey(wm => wm.WorkspaceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(wm => wm.InvitedBy)
                    .WithMany()
                    .HasForeignKey(wm => wm.InvitedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
