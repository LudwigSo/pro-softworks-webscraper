using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driven.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddClaimedByToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimedBy",
                table: "Project",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimedBy",
                table: "Project");
        }
    }
}
