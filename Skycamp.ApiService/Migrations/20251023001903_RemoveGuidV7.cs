using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGuidV7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkspaceUsers",
                schema: "projectmgmt",
                table: "WorkspaceUsers");

            migrationBuilder.DropIndex(
                name: "IX_WorkspaceUsers_WorkspaceId",
                schema: "projectmgmt",
                table: "WorkspaceUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                schema: "projectmgmt",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "projectmgmt",
                table: "WorkspaceUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "projectmgmt",
                table: "ProjectUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkspaceUsers",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                columns: new[] { "WorkspaceId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                schema: "projectmgmt",
                table: "ProjectUsers",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_JoinedUtc",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "JoinedUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkspaceUsers",
                schema: "projectmgmt",
                table: "WorkspaceUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                schema: "projectmgmt",
                table: "ProjectUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUsers_JoinedUtc",
                schema: "projectmgmt",
                table: "ProjectUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "projectmgmt",
                table: "ProjectUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkspaceUsers",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_WorkspaceId",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                column: "WorkspaceId");
        }
    }
}
