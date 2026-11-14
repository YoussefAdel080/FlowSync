using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowSync.Application.Migrations
{
    /// <inheritdoc />
    public partial class replaceInvitedByUserRelationToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceInvitations_Users_InvitedById",
                table: "WorkspaceInvitations");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceInvitations_WorkspaceMembers_InvitedById",
                table: "WorkspaceInvitations",
                column: "InvitedById",
                principalTable: "WorkspaceMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceInvitations_WorkspaceMembers_InvitedById",
                table: "WorkspaceInvitations");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceInvitations_Users_InvitedById",
                table: "WorkspaceInvitations",
                column: "InvitedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
