using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspNetWebApi.Migrations
{
    public partial class CreateTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NumberOfLegs = table.Column<int>(nullable: false),
                    LivesInId = table.Column<int>(nullable: false)
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

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "England" });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "France" });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Germany" });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "LivesInId", "Name", "NumberOfLegs" },
                values: new object[] { 1, 1, "Dog", 4 });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "LivesInId", "Name", "NumberOfLegs" },
                values: new object[] { 2, 2, "Cat", 4 });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "LivesInId", "Name", "NumberOfLegs" },
                values: new object[] { 3, 3, "Sloth", 2 });

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
                name: "Countries");
        }
    }
}
