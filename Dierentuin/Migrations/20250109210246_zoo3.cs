using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dierentuin.Migrations
{
    /// <inheritdoc />
    public partial class zoo3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnclosureIds",
                table: "Zoos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnclosureIds",
                table: "Zoos");
        }
    }
}
