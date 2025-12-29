using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaOrderapp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pizzas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BasePrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IngredientsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pizzas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PizzaSizes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Multiplier = table.Column<double>(type: "REAL", nullable: false),
                    PizzaId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PizzaSizes_Pizzas_PizzaId",
                        column: x => x.PizzaId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SelectedPizzaId = table.Column<string>(type: "TEXT", nullable: true),
                    SelectedSizeId = table.Column<string>(type: "TEXT", nullable: true),
                    SelectedToppingsJson = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "[]"),
                    TotalPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomerInfoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_CustomerInfos_CustomerInfoId",
                        column: x => x.CustomerInfoId,
                        principalTable: "CustomerInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_PizzaSizes_SelectedSizeId",
                        column: x => x.SelectedSizeId,
                        principalTable: "PizzaSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Pizzas_SelectedPizzaId",
                        column: x => x.SelectedPizzaId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInfos_Email",
                table: "CustomerInfos",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerInfoId",
                table: "Orders",
                column: "CustomerInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SelectedPizzaId",
                table: "Orders",
                column: "SelectedPizzaId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SelectedSizeId",
                table: "Orders",
                column: "SelectedSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaSizes_PizzaId",
                table: "PizzaSizes",
                column: "PizzaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "CustomerInfos");

            migrationBuilder.DropTable(
                name: "PizzaSizes");

            migrationBuilder.DropTable(
                name: "Pizzas");
        }
    }
}
