using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectActivities",
                schema: "projectmgmt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserAvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OccurredUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectActivities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectActivities_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "projectmgmt",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActivities_ProjectId_OccurredUtc",
                schema: "projectmgmt",
                table: "ProjectActivities",
                columns: new[] { "ProjectId", "OccurredUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActivities_UserId",
                schema: "projectmgmt",
                table: "ProjectActivities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectActivities",
                schema: "projectmgmt");
        }
    }
}
