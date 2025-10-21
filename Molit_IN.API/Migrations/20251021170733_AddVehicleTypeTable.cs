using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Molit_IN.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleType",
                columns: table => new
                {
                    IdVehicleType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DscriptionVehicle = table.Column<string>(type: "varchar(100)", nullable: true),
                    ItemCode = table.Column<string>(type: "varchar(100)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Registro activo"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Usuario que creo el registro"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Fecha y hora de creación del registro"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "Ultimo usuario que modificó el registro"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Ultima fecha y hora de actualización del registro")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleType", x => x.IdVehicleType);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleType");
        }
    }
}
