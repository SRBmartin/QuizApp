using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Quiz");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "integer", nullable: false),
                    TimeInSeconds = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "Quiz",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "Quiz",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attempts",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TotalScore = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attempts_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "Quiz",
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attempts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Quiz",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "Quiz",
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "Quiz",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizTags",
                schema: "Quiz",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizTags", x => new { x.QuizId, x.TagId });
                    table.ForeignKey(
                        name: "FK_QuizTags_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalSchema: "Quiz",
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Quiz",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptItems",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardedScore = table.Column<int>(type: "integer", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    AnsweredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttemptItems_Attempts_AttemptId",
                        column: x => x.AttemptId,
                        principalSchema: "Quiz",
                        principalTable: "Attempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptItems_QuizQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Quiz",
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionChoices",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionChoices_QuizQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Quiz",
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionTexts",
                schema: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionTexts_QuizQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Quiz",
                        principalTable: "QuizQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptItemTexts",
                schema: "Quiz",
                columns: table => new
                {
                    AttemptItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmittedText = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptItemTexts", x => x.AttemptItemId);
                    table.ForeignKey(
                        name: "FK_AttemptItemTexts_AttemptItems_AttemptItemId",
                        column: x => x.AttemptItemId,
                        principalSchema: "Quiz",
                        principalTable: "AttemptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttemptItemChoices",
                schema: "Quiz",
                columns: table => new
                {
                    AttemptItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChoiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttemptItemChoices", x => new { x.AttemptItemId, x.ChoiceId });
                    table.ForeignKey(
                        name: "FK_AttemptItemChoices_AttemptItems_AttemptItemId",
                        column: x => x.AttemptItemId,
                        principalSchema: "Quiz",
                        principalTable: "AttemptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttemptItemChoices_QuizQuestionChoices_ChoiceId",
                        column: x => x.ChoiceId,
                        principalSchema: "Quiz",
                        principalTable: "QuizQuestionChoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttemptItemChoices_ChoiceId",
                schema: "Quiz",
                table: "AttemptItemChoices",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttemptItems_AttemptId_QuestionId",
                schema: "Quiz",
                table: "AttemptItems",
                columns: new[] { "AttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttemptItems_QuestionId",
                schema: "Quiz",
                table: "AttemptItems",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_QuizId",
                schema: "Quiz",
                table: "Attempts",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_UserId",
                schema: "Quiz",
                table: "Attempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionChoices_QuestionId",
                schema: "Quiz",
                table: "QuizQuestionChoices",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_CreatedById",
                schema: "Quiz",
                table: "QuizQuestions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizId",
                schema: "Quiz",
                table: "QuizQuestions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionTexts_QuestionId",
                schema: "Quiz",
                table: "QuizQuestionTexts",
                column: "QuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizTags_TagId",
                schema: "Quiz",
                table: "QuizTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CreatedById",
                schema: "Quiz",
                table: "Quizzes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                schema: "Quiz",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Quiz",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "Quiz",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttemptItemChoices",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "AttemptItemTexts",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "QuizQuestionTexts",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "QuizTags",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "QuizQuestionChoices",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "AttemptItems",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "Attempts",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "QuizQuestions",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "Quizzes",
                schema: "Quiz");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Quiz");
        }
    }
}
