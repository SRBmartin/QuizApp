using MediatR;
using QuizApp.Application.Common.Result;
using QuizApp.Application.DTOs.Quizes;

namespace QuizApp.Application.Features.Quizes.GetGlobalLeaderBoard;

public record GetGlobalLeaderboardQuery
(
    string? Period = "all",
    int Take = 50
) : IRequest<Result<GlobalLeaderboardDto>>;