using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Infrastructure.Configuration;
using QuizApp.Infrastructure.Persistence.Context;
using QuizApp.Infrastructure.Services.MinIo;

namespace QuizApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddMinIo(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("QuizDb")
            ?? throw new InvalidOperationException("Connection string 'QuizDb' not found.");

        services.AddDbContext<QuizDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(QuizDbContext).Assembly.FullName);
            });
        });

        return services;
    }

    private static IServiceCollection AddMinIo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MinioImageStorageConfiguration>()
            .Bind(configuration.GetSection(MinioImageStorageConfiguration.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageStorage, MinIoImageStorage>();

        return services;
    }

}
