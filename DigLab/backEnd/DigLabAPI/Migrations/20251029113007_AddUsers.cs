using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigLabAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 926, DateTimeKind.Utc).AddTicks(9958));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(285));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(288));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(290));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(292));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(294));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(296));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(297));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(299));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 927, DateTimeKind.Utc).AddTicks(301));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(7986));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8309));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8311));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8313));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8314));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8316));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8318));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8319));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8321));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 11, 30, 6, 921, DateTimeKind.Utc).AddTicks(8323));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAtUtc", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 10, 29, 11, 30, 7, 177, DateTimeKind.Utc).AddTicks(7166), "$2a$11$wTFMl78kYDydKBdoQNBpNufOJmCaEInQIGMJmlH/YARd.tP0A1Yje", "admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3329));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3646));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3648));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3650));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3653));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3655));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3657));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3659));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3734));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3736));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(6903));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7484));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7486));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7488));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7589));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7592));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7593));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7595));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7597));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 11, 16, 13, 28, 769, DateTimeKind.Utc).AddTicks(7598));
        }
    }
}
