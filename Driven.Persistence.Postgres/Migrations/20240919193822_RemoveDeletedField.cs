using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driven.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeletedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "RemovedAt",
                table: "Project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Project",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedAt",
                table: "Project",
                type: "timestamp",
                nullable: true);
        }
    }
}
