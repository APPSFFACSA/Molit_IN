using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Molit_IN.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPilotsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pilots",
                columns: table => new
                {
                    PilotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(254)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicensePhoto = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    NamePhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Registro activo"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Usuario que creo el registro"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Fecha y hora de creación del registro"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "Ultimo usuario que modificó el registro"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Ultima fecha y hora de actualización del registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.PilotId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pilots");
        }
    }
}
