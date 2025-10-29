using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigLabAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExtendUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "HprNumber",
                table: "Users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Profession",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "WorkerId",
                table: "Users",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6308));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6636));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6639));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6641));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6643));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6645));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6647));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6649));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6651));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 96, DateTimeKind.Utc).AddTicks(6652));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(4701));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5029));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5031));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5033));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5035));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5036));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5038));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5040));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5042));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 10, 29, 13, 20, 56, 91, DateTimeKind.Utc).AddTicks(5043));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "FirstName", "HprNumber", "LastName", "PasswordHash", "Profession", "WorkerId" },
                values: new object[] { new DateTime(2025, 10, 29, 13, 20, 56, 329, DateTimeKind.Utc).AddTicks(8555), "System", "", "Administrator", "$2a$11$CJsHXylAuK78esxFM7vjN..35Jnu8CwIJzEoB2sMAzDaKcAuKm4xy", 3, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_WorkerId",
                table: "Users",
                column: "WorkerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_WorkerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HprNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Profession",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "Users");

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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 29, 11, 30, 7, 177, DateTimeKind.Utc).AddTicks(7166), "$2a$11$wTFMl78kYDydKBdoQNBpNufOJmCaEInQIGMJmlH/YARd.tP0A1Yje" });
        }
    }
}
