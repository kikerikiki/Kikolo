using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kikolo_v1.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fragenpacks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fragenpacks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    FragenPack = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FragenpacksQuestions",
                columns: table => new
                {
                    FragenpackID = table.Column<int>(type: "int", nullable: false),
                    QuestionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Fragenpa__23E53155D3324B72", x => new { x.FragenpackID, x.QuestionID });
                    table.ForeignKey(
                        name: "FK__Fragenpac__Frage__3D5E1FD2",
                        column: x => x.FragenpackID,
                        principalTable: "Fragenpacks",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK__Fragenpac__Quest__3E52440B",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FragenpacksQuestions_QuestionID",
                table: "FragenpacksQuestions",
                column: "QuestionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FragenpacksQuestions");

            migrationBuilder.DropTable(
                name: "Fragenpacks");

            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
