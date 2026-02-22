using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProQuote.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteScoringTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.CreateTable(
                name: "QuoteScoringTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RfqId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceWeight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    LeadTimeWeight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CoverageWeight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteScoringTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteScoringTemplates_Rfqs_RfqId",
                        column: x => x.RfqId,
                        principalTable: "Rfqs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteScoringTemplates_RfqId",
                table: "QuoteScoringTemplates",
                column: "RfqId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.DropTable(
                name: "QuoteScoringTemplates");
        }
    }
}
