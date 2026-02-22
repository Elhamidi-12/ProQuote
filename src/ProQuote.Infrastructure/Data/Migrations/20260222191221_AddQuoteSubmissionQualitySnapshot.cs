using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProQuote.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteSubmissionQualitySnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubmissionCommercialScore",
                table: "Quotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionCompletenessScore",
                table: "Quotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionLeadTimeScore",
                table: "Quotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionQualityScore",
                table: "Quotes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionQualityScoredAt",
                table: "Quotes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionCommercialScore",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "SubmissionCompletenessScore",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "SubmissionLeadTimeScore",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "SubmissionQualityScore",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "SubmissionQualityScoredAt",
                table: "Quotes");
        }
    }
}
