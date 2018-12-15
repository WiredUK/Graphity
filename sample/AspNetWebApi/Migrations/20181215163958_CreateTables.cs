using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetWebApi.Migrations
{
    // ReSharper disable once UnusedMember.Global
    public partial class CreateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NumberOfLegs = table.Column<short>(),
                    AnimalType = table.Column<int>(),
                    LivesInId = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Countries_LivesInId",
                        column: x => x.LivesInId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryProperties",
                columns: table => new
                {
                    CountryId = table.Column<int>(),
                    Population = table.Column<long>(),
                    TaxRate = table.Column<decimal>(),
                    HasMonarchy = table.Column<bool>(),
                    LastTimeWonWorldCup = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryProperties", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_CountryProperties_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "England" },
                    { 2, "France" },
                    { 3, "Germany" },
                    { 4, "Australia" }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "AnimalType", "LivesInId", "Name", "NumberOfLegs" },
                values: new object[,]
                {
                    { 1, 0, 1, "Dog", (short)4 },
                    { 2, 0, 2, "Cat", (short)4 },
                    { 3, 0, 3, "Sloth", (short)2 },
                    { 4, 1, 4, "Snake", (short)0 }
                });

            migrationBuilder.InsertData(
                table: "CountryProperties",
                columns: new[] { "CountryId", "HasMonarchy", "LastTimeWonWorldCup", "Population", "TaxRate" },
                values: new object[,]
                {
                    { 1, true, new DateTime(1966, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 55619400L, 0.2m },
                    { 2, false, new DateTime(2018, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 67348000L, 0.2m },
                    { 3, true, new DateTime(2014, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 82800000L, 0.19m },
                    { 4, true, null, 25155300L, 0.1m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_LivesInId",
                table: "Animals",
                column: "LivesInId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "CountryProperties");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
