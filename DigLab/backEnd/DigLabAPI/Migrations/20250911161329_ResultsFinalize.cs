using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigLabAPI.Migrations
{
    /// <inheritdoc />
    public partial class ResultsFinalize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AnyOverridden",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequisitionPdfPath",
                table: "Orders",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ResultsPdfPath",
                table: "Orders",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "ResultsSaved",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OrderResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Diagnosis = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Auto = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Final = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Overridden = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderResults_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3329), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3646), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3648), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3650), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3653), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3655), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3657), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3659), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3734), null, null, false });

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AnyOverridden", "CreatedAtUtc", "RequisitionPdfPath", "ResultsPdfPath", "ResultsSaved" },
                values: new object[] { false, new DateTime(2025, 9, 11, 16, 13, 28, 775, DateTimeKind.Utc).AddTicks(3736), null, null, false });

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

            migrationBuilder.CreateIndex(
                name: "IX_OrderResults_OrderId",
                table: "OrderResults",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderResults");

            migrationBuilder.DropColumn(
                name: "AnyOverridden",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RequisitionPdfPath",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ResultsPdfPath",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ResultsSaved",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9692));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9985));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9987));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9989));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9991));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9992));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 379, DateTimeKind.Utc).AddTicks(9994));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 380, DateTimeKind.Utc).AddTicks(65));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 380, DateTimeKind.Utc).AddTicks(66));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 380, DateTimeKind.Utc).AddTicks(68));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7415));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7731));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7733));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7735));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7737));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7739));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7740));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7742));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7744));

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAtUtc",
                value: new DateTime(2025, 9, 9, 19, 48, 24, 378, DateTimeKind.Utc).AddTicks(7745));
        }
    }
}
