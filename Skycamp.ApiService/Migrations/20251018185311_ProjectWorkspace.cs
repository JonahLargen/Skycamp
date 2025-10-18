using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class ProjectWorkspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Workspaces_WorkspaceId",
                schema: "projectmgmt",
                table: "ProjectUsers");

            migrationBuilder.RenameColumn(
                name: "WorkspaceId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUsers_WorkspaceId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                newName: "IX_ProjectUsers_ProjectId");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkspaceId",
                schema: "projectmgmt",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Projects_WorkspaceId",
                schema: "projectmgmt",
                table: "Projects",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Workspaces_WorkspaceId",
                schema: "projectmgmt",
                table: "Projects",
                column: "WorkspaceId",
                principalSchema: "projectmgmt",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Projects_ProjectId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "ProjectId",
                principalSchema: "projectmgmt",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Workspaces_WorkspaceId",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Projects_ProjectId",
                schema: "projectmgmt",
                table: "ProjectUsers");

            migrationBuilder.DropIndex(
                name: "IX_Projects_WorkspaceId",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                newName: "WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUsers_ProjectId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                newName: "IX_ProjectUsers_WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Workspaces_WorkspaceId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "WorkspaceId",
                principalSchema: "projectmgmt",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
