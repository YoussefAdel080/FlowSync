using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowSync.Application.Migrations
{
    /// <inheritdoc />
    public partial class WorkspaceMembershipAndInvitationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_Users_InvitedById",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_InvitedById",
                table: "WorkspaceMembers");

            migrationBuilder.DropColumn(
                name: "InvitedById",
                table: "WorkspaceMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkspaceInvitationId",
                table: "WorkspaceMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "WorkspaceInvitations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_WorkspaceInvitationId",
                table: "WorkspaceMembers",
                column: "WorkspaceInvitationId",
                unique: true,
                filter: "[WorkspaceInvitationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvitations_Token",
                table: "WorkspaceInvitations",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_WorkspaceInvitations_WorkspaceInvitationId",
                table: "WorkspaceMembers",
                column: "WorkspaceInvitationId",
                principalTable: "WorkspaceInvitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceMembers_WorkspaceInvitations_WorkspaceInvitationId",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceMembers_WorkspaceInvitationId",
                table: "WorkspaceMembers");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceInvitations_Token",
                table: "WorkspaceInvitations");

            migrationBuilder.DropColumn(
                name: "WorkspaceInvitationId",
                table: "WorkspaceMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "InvitedById",
                table: "WorkspaceMembers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "WorkspaceInvitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_InvitedById",
                table: "WorkspaceMembers",
                column: "InvitedById");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceMembers_Users_InvitedById",
                table: "WorkspaceMembers",
                column: "InvitedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
