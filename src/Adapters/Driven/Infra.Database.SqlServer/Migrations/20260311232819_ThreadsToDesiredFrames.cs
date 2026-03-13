using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infra.Database.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ThreadsToDesiredFrames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Threads",
                table: "Plans");

            migrationBuilder.AddColumn<int>(
                name: "DesiredFrames",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "CreatedAt", "DesiredFrames", "ImageQuality", "MaxDurationInSeconds", "MaxSizeInMegaBytes", "Name", "Price" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 11, 20, 28, 18, 937, DateTimeKind.Local).AddTicks(9372), 2, "Hd", "20", "200", "Default", 9.99m },
                    { 2, new DateTime(2026, 3, 11, 20, 28, 18, 937, DateTimeKind.Local).AddTicks(9392), 4, "FullHd", "1200", "2000", "Standard", 19.99m },
                    { 3, new DateTime(2026, 3, 11, 20, 28, 18, 937, DateTimeKind.Local).AddTicks(9393), 8, "FourK", "3600", "10000", "Premium", 29.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "DesiredFrames",
                table: "Plans");

            migrationBuilder.AddColumn<string>(
                name: "Threads",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
