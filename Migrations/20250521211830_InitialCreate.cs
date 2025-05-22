using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScrapeWise_Intelligent_Web_Scraping_Dashboard_ASP.NET_Core_MVC_.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ConfigProfiles",
                columns: table => new
                {
                    ConfigProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DelayBetweenRequests = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigProfiles", x => x.ConfigProfileId);
                    table.ForeignKey(
                        name: "FK_ConfigProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScrapingJobs",
                columns: table => new
                {
                    ScrapingJobId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CssSelector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingJobs", x => x.ScrapingJobId);
                    table.ForeignKey(
                        name: "FK_ScrapingJobs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScrapingResults",
                columns: table => new
                {
                    ScrapingResultId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExtractedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScrapedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScrapingJobId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapingResults", x => x.ScrapingResultId);
                    table.ForeignKey(
                        name: "FK_ScrapingResults_ScrapingJobs_ScrapingJobId",
                        column: x => x.ScrapingJobId,
                        principalTable: "ScrapingJobs",
                        principalColumn: "ScrapingJobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigProfiles_UserId",
                table: "ConfigProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScrapingJobs_UserId",
                table: "ScrapingJobs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapingResults_ScrapingJobId",
                table: "ScrapingResults",
                column: "ScrapingJobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigProfiles");

            migrationBuilder.DropTable(
                name: "ScrapingResults");

            migrationBuilder.DropTable(
                name: "ScrapingJobs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
