using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurationIndexesChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attempts_UserId",
                schema: "Quiz",
                table: "Attempts");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedText",
                schema: "Quiz",
                table: "AttemptItemTexts",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_StartedAt",
                schema: "Quiz",
                table: "Attempts",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_UserId_QuizId_Status",
                schema: "Quiz",
                table: "Attempts",
                columns: new[] { "UserId", "QuizId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attempts_StartedAt",
                schema: "Quiz",
                table: "Attempts");

            migrationBuilder.DropIndex(
                name: "IX_Attempts_UserId_QuizId_Status",
                schema: "Quiz",
                table: "Attempts");

            migrationBuilder.AlterColumn<string>(
                name: "SubmittedText",
                schema: "Quiz",
                table: "AttemptItemTexts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1024)",
                oldMaxLength: 1024);

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_UserId",
                schema: "Quiz",
                table: "Attempts",
                column: "UserId");
        }
    }
}
