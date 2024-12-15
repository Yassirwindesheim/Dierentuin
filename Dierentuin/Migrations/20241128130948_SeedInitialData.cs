using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dierentuin.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mammals" },
                    { 2, "Birds" },
                    { 3, "Reptiles" }
                });

            migrationBuilder.InsertData(
                table: "Enclosures",
                columns: new[] { "Id", "Climate", "HabitatType", "Name", "SecurityLevel", "Size", "ZooId" },
                values: new object[,]
                {
                    { 1, 0, 8, "Savanna Exhibit", 1, 1000.0, null },
                    { 2, 1, 1, "Aviary", 0, 500.0, null },
                    { 3, 2, 2, "Reptile House", 2, 300.0, null }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "ActivityPattern", "AnimalId", "CategoryId", "Diet", "EnclosureId", "Name", "SecurityRequirement", "Size", "SpaceRequirement", "Species", "ZooId" },
                values: new object[,]
                {
                    { 1, 2, null, 1, 0, 1, "Lion", 2, 4, 200.0, "Panthera leo", null },
                    { 2, 0, null, 2, 0, 2, "Eagle", 1, 3, 100.0, "Aquila chrysaetos", null },
                    { 3, 1, null, 3, 0, 3, "Python", 2, 4, 50.0, "Python reticulatus", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
