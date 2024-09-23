using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Driven.Persistence.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataForTagsAndKeywords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, "Architecture" },
                    { 3, "UI" }
                });

            migrationBuilder.InsertData(
                table: "Keyword",
                columns: new[] { "Id", "TagId", "Value" },
                values: new object[,]
                {
                    { 1, 1, "c#" },
                    { 2, 1, "csharp" },
                    { 3, 1, "c-sharp" },
                    { 4, 1, "aspnet" },
                    { 5, 1, "dotnet" },
                    { 6, 2, "ddd" },
                    { 7, 2, "domaindrivendesign" },
                    { 8, 2, "domain-driven-design" },
                    { 9, 2, "eric evans" },
                    { 10, 2, "ericevans" },
                    { 11, 2, "eric-evans" },
                    { 12, 2, "portsandadapters" },
                    { 13, 2, "ports&adapters" },
                    { 14, 2, "ports-and-adapters" },
                    { 15, 2, "hexagonal" },
                    { 16, 2, "clean" },
                    { 17, 2, "layered" },
                    { 18, 3, "vuejs" },
                    { 19, 3, "vue.js" },
                    { 20, 3, "vuetify" },
                    { 21, 3, "javascript" },
                    { 22, 3, "typscript" },
                    { 23, 3, "figma" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Keyword",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
