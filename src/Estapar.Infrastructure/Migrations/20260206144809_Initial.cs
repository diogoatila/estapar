using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estapar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarageSectors",
                columns: table => new
                {
                    SectorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageSectors", x => x.SectorCode);
                });

            migrationBuilder.CreateTable(
                name: "ParkingSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SectorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SpotId = table.Column<int>(type: "int", nullable: false),
                    EntryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PricePerHourApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountCharged = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParkingSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SectorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Lat = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Lng = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_EntryTime",
                table: "ParkingSessions",
                column: "EntryTime");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_ExitTime",
                table: "ParkingSessions",
                column: "ExitTime");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_LicensePlate",
                table: "ParkingSessions",
                column: "LicensePlate",
                unique: true,
                filter: "[ExitTime] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSessions_SectorCode",
                table: "ParkingSessions",
                column: "SectorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Spots_SectorCode_IsOccupied",
                table: "Spots",
                columns: new[] { "SectorCode", "IsOccupied" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarageSectors");

            migrationBuilder.DropTable(
                name: "ParkingSessions");

            migrationBuilder.DropTable(
                name: "Spots");
        }
    }
}
