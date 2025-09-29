using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;

namespace QuizApp.Application.Features.Quizes.GetLeaderboard;

public record GetQuizLeaderboardQuery(
    Guid QuizId,
    string? Period, //"all" | "month" | "week"
    int Take
) : IRequest<Result<LeaderboardDto>>;