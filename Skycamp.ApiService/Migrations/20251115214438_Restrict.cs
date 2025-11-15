using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class Restrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_UserId",
                schema: "projectmgmt",
                table: "ProjectActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectActivities_Projects_ProjectId",
                schema: "projectmgmt",
                table: "ProjectActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_UserId",
                schema: "projectmgmt",
                table: "ProjectActivities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectActivities_Projects_ProjectId",
                schema: "projectmgmt",
                table: "ProjectActivities",
                column: "ProjectId",
                principalSchema: "projectmgmt",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_UserId",
                schema: "projectmgmt",
                table: "ProjectActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectActivities_Projects_ProjectId",
                schema: "projectmgmt",
                table: "ProjectActivities");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectActivities_AspNetUsers_UserId",
                schema: "projectmgmt",
                table: "ProjectActivities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectActivities_Projects_ProjectId",
                schema: "projectmgmt",
                table: "ProjectActivities",
                column: "ProjectId",
                principalSchema: "projectmgmt",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
