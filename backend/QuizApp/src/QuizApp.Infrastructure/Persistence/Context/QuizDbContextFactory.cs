using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuizApp.Infrastructure.Persistence.Context;

public class QuizDbContextFactory : IDesignTimeDbContextFactory<QuizDbContext>
{
    public QuizDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<QuizDbContext>();

        var cs = Environment.GetEnvironmentVariable("QUIZAPP_MIGRATIONS_CS")
                 ?? "Host=localhost;Port=5432;Database=quizdb;Username=quiz;Password=quizpwd";

        builder.UseNpgsql(cs);

        return new QuizDbContext(builder.Options);
    }
}
