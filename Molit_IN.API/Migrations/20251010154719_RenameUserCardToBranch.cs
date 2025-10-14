using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Molit_IN.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserCardToBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardName",
                table: "Users",
                newName: "BranchName");

            migrationBuilder.RenameColumn(
                name: "CardCode",
                table: "Users",
                newName: "BranchCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BranchName",
                table: "Users",
                newName: "CardName");

            migrationBuilder.RenameColumn(
                name: "BranchCode",
                table: "Users",
                newName: "CardCode");
        }
    }
}
