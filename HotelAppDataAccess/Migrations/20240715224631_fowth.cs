using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelAppLibrary.Migrations
{
    /// <inheritdoc />
    public partial class fowth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Hotels_HotelId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "HotelId",
                table: "Bookings",
                newName: "RoomTypeId");

            migrationBuilder.AddColumn<int>(
                name: "HotelModelHotelId",
                table: "Rooms",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelModelHotelId",
                table: "Rooms",
                column: "HotelModelHotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomTypeId",
                table: "Bookings",
                column: "RoomTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_RoomTypes_RoomTypeId",
                table: "Bookings",
                column: "RoomTypeId",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_HotelModelHotelId",
                table: "Rooms",
                column: "HotelModelHotelId",
                principalTable: "Hotels",
                principalColumn: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_RoomTypes_RoomTypeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_HotelModelHotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_HotelModelHotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomTypeId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "HotelModelHotelId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "RoomTypeId",
                table: "Bookings",
                newName: "HotelId");

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews",
                column: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Hotels_HotelId",
                table: "Reviews",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "HotelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "HotelId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
