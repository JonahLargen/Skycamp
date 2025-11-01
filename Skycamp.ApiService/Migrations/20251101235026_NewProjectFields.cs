using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skycamp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class NewProjectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedUtc",
                schema: "projectmgmt",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                schema: "projectmgmt",
                table: "Projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Progress",
                schema: "projectmgmt",
                table: "Projects",
                type: "decimal(5,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "projectmgmt",
                table: "Projects",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                schema: "projectmgmt",
                table: "Projects",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedUtc",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Progress",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "projectmgmt",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StartDate",
                schema: "projectmgmt",
                table: "Projects");
        }
    }
}
