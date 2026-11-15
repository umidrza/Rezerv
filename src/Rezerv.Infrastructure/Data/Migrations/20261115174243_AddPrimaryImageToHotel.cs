using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rezerv.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryImageToHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "HotelImages");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryImageId",
                table: "Hotels",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_PrimaryImageId",
                table: "Hotels",
                column: "PrimaryImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_HotelImages_PrimaryImageId",
                table: "Hotels",
                column: "PrimaryImageId",
                principalTable: "HotelImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_HotelImages_PrimaryImageId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_PrimaryImageId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "PrimaryImageId",
                table: "Hotels");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "HotelImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
