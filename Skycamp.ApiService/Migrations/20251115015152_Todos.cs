using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class Todos : Migration
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
                table: "Projects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Todos",
                schema: "projectmgmt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PrimaryAssigneeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todos_AspNetUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Todos_AspNetUsers_PrimaryAssigneeId",
                        column: x => x.PrimaryAssigneeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Todos_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "projectmgmt",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CreateUserId",
                schema: "projectmgmt",
                table: "Todos",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_PrimaryAssigneeId",
                schema: "projectmgmt",
                table: "Todos",
                column: "PrimaryAssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_ProjectId",
                schema: "projectmgmt",
                table: "Todos",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Projects",
                column: "CreateUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_AspNetUsers_CreateUserId",
                schema: "projectmgmt",
                table: "Workspaces",
                column: "CreateUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                name: "Todos",
                schema: "projectmgmt");

            migrationBuilder.AlterColumn<string>(
                name: "CreateUserId",
                schema: "projectmgmt",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
    }
}
