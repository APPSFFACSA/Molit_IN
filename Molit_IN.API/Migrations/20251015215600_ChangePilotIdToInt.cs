using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Molit_IN.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangePilotIdToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PilotId",
                table: "Copilots",
                type: "int",
                nullable: true,
                comment: "Piloto al que esta asignado el Auxiliar",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Piloto al que esta asignado el Auxiliar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PilotId",
                table: "Copilots",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Piloto al que esta asignado el Auxiliar",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Piloto al que esta asignado el Auxiliar");
        }
    }
}
