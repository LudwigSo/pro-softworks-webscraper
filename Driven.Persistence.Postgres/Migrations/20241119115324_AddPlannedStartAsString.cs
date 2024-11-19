using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driven.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddPlannedStartAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlannedStartAsString",
                table: "Project",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlannedStartAsString",
                table: "Project");
        }
    }
}
