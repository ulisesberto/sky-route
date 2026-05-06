using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyRoute.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingReference = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    DepartureTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CabinClass = table.Column<int>(type: "int", nullable: false),
                    Passengers = table.Column<int>(type: "int", nullable: false),
                    PerPassengerPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    IsInternational = table.Column<bool>(type: "bit", nullable: false),
                    PassengerFullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PassengerEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PassengerDocumentNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingReference",
                table: "Bookings",
                column: "BookingReference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
