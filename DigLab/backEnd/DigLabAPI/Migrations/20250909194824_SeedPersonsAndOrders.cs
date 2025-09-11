using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DigLabAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedPersonsAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAtUtc", "Date", "DiagnosesJson", "LabNumber", "Name", "Personnummer", "Time" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9692), new DateOnly(2025, 9, 9), "[\"Dengue\"]", "LAB-20250909-AAA11111", "Ola Nordmann", "01010112345", new TimeOnly(9, 15, 0) },
                    { 2, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9985), new DateOnly(2025, 9, 9), "[\"Malaria\"]", "LAB-20250909-BBB22222", "Kari Nordmann", "02020223456", new TimeOnly(9, 30, 0) },
                    { 3, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9987), new DateOnly(2025, 9, 9), "[\"TBE\",\"Dengue\"]", "LAB-20250909-CCC33333", "Per Hansen", "03030334567", new TimeOnly(10, 0, 0) },
                    { 4, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9989), new DateOnly(2025, 9, 9), "[\"Hantavirus – Puumalavirus (PuV)\"]", "LAB-20250909-DDD44444", "Anne Larsen", "04040445678", new TimeOnly(10, 30, 0) },
                    { 5, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9991), new DateOnly(2025, 9, 9), "[\"Dengue\"]", "LAB-20250909-EEE55555", "Marius Bakke", "05050556789", new TimeOnly(11, 0, 0) },
                    { 6, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9992), new DateOnly(2025, 9, 9), "[\"Malaria\",\"TBE\"]", "LAB-20250909-FFF66666", "Ingrid Lie", "06060667890", new TimeOnly(11, 15, 0) },
                    { 7, new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9994), new DateOnly(2025, 9, 9), "[\"TBE\"]", "LAB-20250909-GGG77777", "Jonas Moen", "07070778901", new TimeOnly(11, 45, 0) },
                    { 8, new DateTime(2025, 9, 9, 19, 48, 24, 380, DateTimeKind.Utc).AddTicks(65), new DateOnly(2025, 9, 9), "[\"Dengue\",\"Malaria\"]", "LAB-20250909-HHH88888", "Camilla Johansen", "08080889012", new TimeOnly(12, 0, 0) },
                    { 9, new DateTime(2025, 9, 9, 19, 48, 24, 380, DateTimeKind.Utc).AddTicks(66), new DateOnly(2025, 9, 9), "[\"Hantavirus – Puumalavirus (PuV)\"]", "LAB-20250909-III99999", "Andreas Solheim", "09090990123", new TimeOnly(12, 30, 0) },
                    { 10, new DateTime(2025, 9, 9, 19, 48, 24, 380, DateTimeKind.Utc).AddTicks(68), new DateOnly(2025, 9, 9), "[\"Malaria\"]", "LAB-20250909-JJJ00000", "Sofie Berg", "10101001234", new TimeOnly(13, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "AddressLine1", "AddressLine2", "City", "CreatedAtUtc", "DateOfBirth", "Email", "FirstName", "LastName", "MiddleName", "Personnummer", "Phone", "PostalCode", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { 1, "Storgata 1", null, "Oslo", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7415), null, "ola@example.com", "Ola", "Nordmann", null, "01010112345", "90000001", "0001", null },
                    { 2, "Parkveien 22", null, "Bergen", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7731), null, "kari@example.com", "Kari", "Nordmann", null, "02020223456", "90000002", "5003", null },
                    { 3, "Markveien 12", null, "Trondheim", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7733), null, "per@example.com", "Per", "Hansen", null, "03030334567", "90000003", "7010", null },
                    { 4, "Solbergveien 7", null, "Tromsø", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7735), null, "anne@example.com", "Anne", "Larsen", null, "04040445678", "90000004", "9008", null },
                    { 5, "Skogveien 14", null, "Gjøvik", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7737), null, "marius@example.com", "Marius", "Bakke", null, "05050556789", "90000005", "2815", null },
                    { 6, "Havnegata 3", null, "Fredrikstad", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7739), null, "ingrid@example.com", "Ingrid", "Lie", null, "06060667890", "90000006", "1606", null },
                    { 7, "Kirkeveien 15", null, "Lillestrøm", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7740), null, "jonas@example.com", "Jonas", "Moen", null, "07070778901", "90000007", "2004", null },
                    { 8, "Elveveien 2", null, "Stavanger", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7742), null, "camilla@example.com", "Camilla", "Johansen", null, "08080889012", "90000008", "4005", null },
                    { 9, "Fjordgata 9", null, "Molde", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7744), null, "andreas@example.com", "Andreas", "Solheim", null, "09090990123", "90000009", "6411", null },
                    { 10, "Torget 8", null, "Hamar", new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7745), null, "sofie@example.com", "Sofie", "Berg", null, "10101001234", "90000010", "2317", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
