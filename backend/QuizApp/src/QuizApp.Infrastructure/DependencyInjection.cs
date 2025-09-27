using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizApp.Application.Abstractions.Identity;
using QuizApp.Application.Abstractions.Storage;
using QuizApp.Domain.Repositories;
using QuizApp.Domain.Repositories.UoW;
using QuizApp.Infrastructure.Configuration;
using QuizApp.Infrastructure.Persistence.Context;
using QuizApp.Infrastructure.Persistence.Repositories;
using QuizApp.Infrastructure.Persistence.Repositories.UoW;
using QuizApp.Infrastructure.Services.Identity;
using QuizApp.Infrastructure.Services.MinIo;

namespace QuizApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddMinIo(configuration);

        services.AddJwtIdentity(configuration);

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

        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IQuizTagRepository, QuizTagRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddMinIo(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MinioImageStorageConfiguration>()
            .Bind(configuration.GetSection(MinioImageStorageConfiguration.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageStorage, MinIoImageStorage>();
        services.AddSingleton<IImageBucketNameProvider, ImageBucketNameProvider>();
        services.AddSingleton<IStorageKeyBuilder, StorageKeyBuilder>();

        return services;
    }

    private static IServiceCollection AddJwtIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtConfiguration>()
            .Bind(configuration.GetSection(JwtConfiguration.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IIdentityService, JwtIdentityService>();

        services.AddSingleton<IPasswordService, BCryptPasswordService>();

        return services;
    }

}
