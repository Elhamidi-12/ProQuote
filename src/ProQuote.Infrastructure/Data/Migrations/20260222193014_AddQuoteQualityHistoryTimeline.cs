using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProQuote.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteQualityHistoryTimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuoteQualityHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OverallScore = table.Column<int>(type: "int", nullable: false),
                    CompletenessScore = table.Column<int>(type: "int", nullable: false),
                    LeadTimeScore = table.Column<int>(type: "int", nullable: false),
                    CommercialScore = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ScoredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteQualityHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteQualityHistory_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteQualityHistory_QuoteId",
                table: "QuoteQualityHistory",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteQualityHistory_ScoredAt",
                table: "QuoteQualityHistory",
                column: "ScoredAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuoteQualityHistory");
        }
    }
}
