using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelAppLibrary.Migrations
{
    /// <inheritdoc />
    public partial class FixRoomModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_RoomTypes_RoomTypeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_HotelModelHotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_HotelModelHotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "HotelModelHotelId",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "HotelId",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeId1",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomTypeId1",
                table: "Bookings",
                column: "RoomTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_RoomTypes_RoomTypeId1",
                table: "Bookings",
                column: "RoomTypeId1",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomTypeId",
                table: "Bookings",
                column: "RoomTypeId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "HotelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_RoomTypes_RoomTypeId1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomTypeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_HotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomTypeId1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "HotelId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomTypeId1",
                table: "Bookings");

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
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_RoomTypes_RoomTypeId",
                table: "Bookings",
                column: "RoomTypeId",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_HotelModelHotelId",
                table: "Rooms",
                column: "HotelModelHotelId",
                principalTable: "Hotels",
                principalColumn: "HotelId");
        }
    }
}
