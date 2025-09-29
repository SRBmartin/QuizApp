using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizApp.Domain.Enums;
using QuizApp.Domain.Repositories.UoW;
using QuizApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace QuizApp.Infrastructure.Background;

public class AttemptExpirationWorker (
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(30);
    private readonly int _batchSize = 200;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var attempts = scope.ServiceProvider.GetRequiredService<IAttemptRepository>();
                var quizzes = scope.ServiceProvider.GetRequiredService<IQuizRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var now = DateTimeOffset.UtcNow;

                var query =
                    attempts.Query()
                        .Where(a => a.Status == AttemptStatus.InProgress);

                var toCheck = await query
                    .OrderBy(a => a.StartedAt)
                    .Take(_batchSize)
                    .ToListAsync(stoppingToken);

                if (toCheck.Count > 0)
                {
                    var quizIds = toCheck.Select(a => a.QuizId).Distinct().ToList();
                    var quizMap = (await Task.WhenAll(quizIds.Select(id => quizzes.FindByIdAsync(id, stoppingToken))))
                        .Where(q => q is not null)
                        .ToDictionary(q => q!.Id, q => q!);

                    foreach (var a in toCheck)
                    {
                        if (!quizMap.TryGetValue(a.QuizId, out var quiz)) continue;

                        var elapsed = (int)(now - a.StartedAt).TotalSeconds;
                        if (elapsed >= quiz.TimeInSeconds)
                        {
                            var total = a.Items.Sum(i => i.AwardedScore);
                            a.SubmittedAt = now;
                            a.Status = AttemptStatus.Completed;
                            a.TotalScore = total;
                        }
                    }

                    await uow.SaveChangesAsync(stoppingToken);
                }
            }
            catch
            {
            }

            try
            {
                await Task.Delay(_period, stoppingToken);
            }
            catch
            {
            }
        }
    }

}
