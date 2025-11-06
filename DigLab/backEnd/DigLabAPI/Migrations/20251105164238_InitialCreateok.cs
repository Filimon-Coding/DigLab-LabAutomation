using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigLabAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateok : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8299));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8623));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8625));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8628));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8630));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8632));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8634));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8636));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8637));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 21, DateTimeKind.Utc).AddTicks(8639));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(7737));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8068));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8070));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8072));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8074));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8075));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8077));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8079));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8080));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 11, 5, 16, 42, 38, 16, DateTimeKind.Utc).AddTicks(8082));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAtUtc", "PasswordHash", "WorkerId" },
                values: new object[] { new DateTime(2025, 11, 5, 16, 42, 38, 264, DateTimeKind.Utc).AddTicks(2565), "$2a$11$pHiVjRTw0Qukfj1mFJSRSOEpQ6it1OtW7o05DNqOfMzRw/9k2P8zC", "1234" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "CreatedAtUtc", "PasswordHash", "WorkerId" },
                values: new object[] { new DateTime(2025, 10, 29, 13, 20, 56, 329, DateTimeKind.Utc).AddTicks(8555), "$2a$11$CJsHXylAuK78esxFM7vjN..35Jnu8CwIJzEoB2sMAzDaKcAuKm4xy", "admin" });
        }
    }
}
