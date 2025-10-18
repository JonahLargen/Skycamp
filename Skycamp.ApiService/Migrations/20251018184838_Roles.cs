using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                schema: "projectmgmt",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<bool>(
                name: "IsAllAccess",
                schema: "projectmgmt",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProjectRoles",
                schema: "projectmgmt",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRoles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceRoles",
                schema: "projectmgmt",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceRoles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUsers",
                schema: "projectmgmt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_ProjectRoles_RoleName",
                        column: x => x.RoleName,
                        principalSchema: "projectmgmt",
                        principalTable: "ProjectRoles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUsers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "projectmgmt",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceUsers",
                schema: "projectmgmt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_WorkspaceRoles_RoleName",
                        column: x => x.RoleName,
                        principalSchema: "projectmgmt",
                        principalTable: "WorkspaceRoles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "projectmgmt",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "projectmgmt",
                table: "ProjectRoles",
                column: "Name",
                values: new object[]
                {
                    "Admin",
                    "Member",
                    "Owner",
                    "Viewer"
                });

            migrationBuilder.InsertData(
                schema: "projectmgmt",
                table: "WorkspaceRoles",
                column: "Name",
                values: new object[]
                {
                    "Admin",
                    "Member",
                    "Owner",
                    "Viewer"
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_RoleName",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_WorkspaceId",
                schema: "projectmgmt",
                table: "ProjectUsers",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_RoleName",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                column: "RoleName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_UserId",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_WorkspaceId",
                schema: "projectmgmt",
                table: "WorkspaceUsers",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Projects",
                column: "CreateUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces",
                column: "CreateUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces");

            migrationBuilder.DropTable(
                name: "ProjectUsers",
                schema: "projectmgmt");

            migrationBuilder.DropTable(
                name: "WorkspaceUsers",
                schema: "projectmgmt");

            migrationBuilder.DropTable(
                name: "ProjectRoles",
                schema: "projectmgmt");

            migrationBuilder.DropTable(
                name: "WorkspaceRoles",
                schema: "projectmgmt");

            migrationBuilder.DropColumn(
                name: "IsAllAccess",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                schema: "projectmgmt",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Projects",
                column: "CreateUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces",
                column: "CreateUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
