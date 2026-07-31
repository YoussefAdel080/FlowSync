using FlowSync.Application.Configuration;
using FlowSync.Application.Contexts;
using FlowSync.Application.Enums;
using FlowSync.Application.Models;
using FlowSync.Contracts.Enums;
using FlowSync.Contracts.Requests;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FlowSync.Application.Repositories
{
    public class WorkspaceInvitationRepository: IWorkspaceInvitationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpOptions _smtpOptions;
        public WorkspaceInvitationRepository(ApplicationDbContext context, IOptions<SmtpOptions> options) {
            _context = context;
            _smtpOptions = options.Value;
        }

        public async Task<bool> InvitationExistsByEmailAsync(string email, Guid workspaceId, CancellationToken token)
        {
            var invitation = await _context.WorkspaceInvitations
                .FirstOrDefaultAsync(i => i.Email == email && i.WorkspaceId == workspaceId, token);
            
            return invitation != null;
        }

        public async Task<bool> CreateWorkspaceInvitationAsync(InviteToWorkspaceRequest request, Guid invitedByUserId, CancellationToken token)
        {
            var workspace = await _context.Workspaces
                .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, token);

            var sender = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == invitedByUserId, token);

            var invitationToken = Guid.NewGuid().ToString();

            if (workspace is null || sender is null) return false;

            await sendInvitationEmail(request.Email, invitationToken, request.Role, workspace.Name, sender.FirstName, token);

            var newInvitation = new WorkspaceInvitation
            {
                Id = Guid.NewGuid(),
                WorkspaceId = request.WorkspaceId,
                Email = request.Email,
                Role = request.Role,
                Token = invitationToken,
                Status = Enums.WorkspaceInvitationStatus.Pending,
                InvitedById = invitedByUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Set expiration date to 7 days from now
            };

            await _context.WorkspaceInvitations.AddAsync(newInvitation);

            await _context.SaveChangesAsync(token);

            return true;
        }

        public async Task<WorkspaceInvitation?> GetWorkspaceInvitationByTokenAndEmailAsync(string InvitationToken, string email, CancellationToken token)
        {
            var invitation = await _context.WorkspaceInvitations
                .AsNoTracking()
                .Include(i => i.Workspace)
                .Include(i => i.InvitedBy)
                .FirstOrDefaultAsync(i => i.Token == InvitationToken && i.Email == email, token);
            
            return invitation;
        }

        public async Task<WorkspaceInvitation?> GetWorkspaceInvitationByIdAsync(Guid id, CancellationToken token)
        {
            var invitation = await _context.WorkspaceInvitations
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id, token);
            return invitation;
        }

        public async Task<bool> AcceptWorkspaceInvitationAsync(AcceptWorkspaceInvitationRequest request, string email, Guid userId, CancellationToken token)
        {
            var invitation = await _context.WorkspaceInvitations
                .FirstOrDefaultAsync(i => i.Token == request.InvitationToken && i.Email == email, token);

            if(invitation is null) return false;

            invitation.Status = Enums.WorkspaceInvitationStatus.Accepted;
            invitation.UpdatedAt = DateTime.UtcNow;

            _context.WorkspaceInvitations.Update(invitation);

            var newMembership = new WorkspaceMember
            {
                Id = Guid.NewGuid(),
                WorkspaceId = invitation.WorkspaceId,
                UserId = userId,
                Role = invitation.Role,
                JoinedAt = DateTime.UtcNow,
                WorkspaceInvitationId = invitation.Id
            };

            _context.WorkspaceMembers.Add(newMembership);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DeclineWorkspaceInvitationAsync(DeclineWorkspaceInvitationRequest request, string email, Guid userId, CancellationToken token)
        {
            var invitation = await _context.WorkspaceInvitations
                .FirstOrDefaultAsync(i => i.Token == request.InvitationToken && i.Email == email, token);

            if (invitation is null) return false;

            invitation.Status = Enums.WorkspaceInvitationStatus.Accepted;
            invitation.UpdatedAt = DateTime.UtcNow;

            _context.WorkspaceInvitations.Update(invitation);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> CancelWorkspaceInvitationAsync(CancelWorkspaceInvitationRequest request, CancellationToken token)
        {
            var invitation = await _context.WorkspaceInvitations
                .FirstOrDefaultAsync(i => i.Id == request.Id, token);

            if (invitation is null) return false;

            invitation.Status = Enums.WorkspaceInvitationStatus.Canceled;
            invitation.UpdatedAt = DateTime.UtcNow;

            _context.WorkspaceInvitations.Update(invitation);
            _context.SaveChanges();

            return true;
        }

        public async Task<IEnumerable<WorkspaceInvitation>> GetPendingWorkspaceInvitationsAsync(GetPendingWorkspaceInvitationsRequest request, CancellationToken token) {
            var invitations = await _context.WorkspaceInvitations
                .AsNoTracking()
                .Where(i => i.WorkspaceId == request.WorkspaceId && i.Status == WorkspaceInvitationStatus.Pending)
                .Include(i => i.Workspace)
                .Include(i => i.InvitedBy)
                .ToListAsync(token);


            return invitations;
        }

        private async Task<bool> sendInvitationEmail(string email, string invitationToken, WorkspaceRole role, string workspaceName, string senderName, CancellationToken token)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _smtpOptions.FromName,
                _smtpOptions.FromEmail));

            message.To.Add(MailboxAddress.Parse(email));

            message.Subject = $"You've been invited to join \"{workspaceName}\" on FlowSync";

            message.Body = new TextPart("plain")
            {
                Text = $"Hello,\r\n\r\n{senderName} has invited you to join the following workspace on FlowSync:\r\n\r\nWorkspace: {workspaceName}\r\nRole: {role}\r\n\r\nTo join the workspace, click the link below:\r\n\r\nhttps://flowsync.com/invitations/{invitationToken}\r\n\r\nThis invitation will expire in 7 days.\r\n\r\nIf you weren't expecting this invitation, you can safely ignore this email.\r\n\r\nThanks,\r\nThe FlowSync Team"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _smtpOptions.Host,
                _smtpOptions.Port,
                SecureSocketOptions.StartTls,
                token);

            await smtp.AuthenticateAsync(
                _smtpOptions.Username,
                _smtpOptions.Password,
                token);

            await smtp.SendAsync(message, token);

            await smtp.DisconnectAsync(true, token);

            return true;
        }

    }
}
