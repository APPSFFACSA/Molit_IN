using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Molit_IN.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCopilotsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Copilots",
                columns: table => new
                {
                    CopilotsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PilotId = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Piloto al que esta asignado el Auxiliar"),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(254)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicensePhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NamePhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Registro activo"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Usuario que creo el registro"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Fecha y hora de creación del registro"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true, comment: "Ultimo usuario que modificó el registro"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Ultima fecha y hora de actualización del registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copilots", x => x.CopilotsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Copilots");
        }
    }
}
