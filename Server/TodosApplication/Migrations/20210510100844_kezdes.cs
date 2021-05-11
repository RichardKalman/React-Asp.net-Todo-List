using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TodosApplication.Migrations
{
    public partial class kezdes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TodoTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Details = table.Column<string>(type: "TEXT", nullable: true),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todo_TodoTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "TodoTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TodoTypes",
                columns: new[] { "Id", "Name", "Order" },
                values: new object[] { 1, "Függőben", 1 });

            migrationBuilder.InsertData(
                table: "TodoTypes",
                columns: new[] { "Id", "Name", "Order" },
                values: new object[] { 2, "Folyamatban", 2 });

            migrationBuilder.InsertData(
                table: "TodoTypes",
                columns: new[] { "Id", "Name", "Order" },
                values: new object[] { 3, "Kész", 3 });

            migrationBuilder.InsertData(
                table: "TodoTypes",
                columns: new[] { "Id", "Name", "Order" },
                values: new object[] { 4, "Elhalasztva", 4 });

            migrationBuilder.InsertData(
                table: "Todo",
                columns: new[] { "Id", "Deadline", "Details", "Name", "Order", "TypeId" },
                values: new object[] { 1, new DateTime(2021, 5, 10, 12, 8, 44, 571, DateTimeKind.Local).AddTicks(4740), "Csak egy teszt", "Teszt", 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Todo_TypeId",
                table: "Todo",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todo");

            migrationBuilder.DropTable(
                name: "TodoTypes");
        }
    }
}
