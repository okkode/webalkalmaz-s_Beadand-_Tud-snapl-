using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GdeWebDB.Migrations
{
    /// <inheritdoc />
    public partial class NotesFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AUDIONOTE",
                columns: table => new
                {
                    AUDIONOTEID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    USERID = table.Column<int>(type: "INTEGER", nullable: false),
                    FILEPATH = table.Column<string>(type: "TEXT", nullable: false),
                    TRANSCRIPT = table.Column<string>(type: "TEXT", nullable: true),
                    NOTEDATE = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MODIFICATIONDATE = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIONOTE", x => x.AUDIONOTEID);
                    table.ForeignKey(
                        name: "FK_AUDIONOTE_T_USER_USERID",
                        column: x => x.USERID,
                        principalTable: "T_USER",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MONTHLYSUMMARY",
                columns: table => new
                {
                    SUMMARYID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    USERID = table.Column<int>(type: "INTEGER", nullable: false),
                    YEAR = table.Column<int>(type: "INTEGER", nullable: false),
                    MONTH = table.Column<int>(type: "INTEGER", nullable: false),
                    SUMMARYTEXT = table.Column<string>(type: "TEXT", nullable: false),
                    LEARNINGREFLECTION = table.Column<string>(type: "TEXT", nullable: false),
                    MODIFICATIONDATE = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MONTHLYSUMMARY", x => x.SUMMARYID);
                    table.ForeignKey(
                        name: "FK_MONTHLYSUMMARY_T_USER_USERID",
                        column: x => x.USERID,
                        principalTable: "T_USER",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NOTE",
                columns: table => new
                {
                    NOTEID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    USERID = table.Column<int>(type: "INTEGER", nullable: false),
                    NOTETITLE = table.Column<string>(type: "TEXT", nullable: false),
                    NOTECONTENT = table.Column<string>(type: "TEXT", nullable: false),
                    NOTEDATE = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MODIFICATIONDATE = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NOTE", x => x.NOTEID);
                    table.ForeignKey(
                        name: "FK_NOTE_T_USER_USERID",
                        column: x => x.USERID,
                        principalTable: "T_USER",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIONOTE_USERID",
                table: "AUDIONOTE",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_MONTHLYSUMMARY_USERID",
                table: "MONTHLYSUMMARY",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_NOTE_USERID",
                table: "NOTE",
                column: "USERID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIONOTE");

            migrationBuilder.DropTable(
                name: "MONTHLYSUMMARY");

            migrationBuilder.DropTable(
                name: "NOTE");
        }
    }
}
