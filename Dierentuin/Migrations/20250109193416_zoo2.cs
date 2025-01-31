using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dierentuin.Migrations
{
    /// <inheritdoc />
    public partial class zoo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnimalIds",
                table: "Zoos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimalIds",
                table: "Zoos");
        }
    }
}
