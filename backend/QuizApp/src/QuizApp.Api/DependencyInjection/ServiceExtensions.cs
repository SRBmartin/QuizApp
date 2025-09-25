using QuizApp.Infrastructure;
using Serilog;

namespace QuizApp.Api.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilogLogger(configuration);

        services.AddInfrastructureServices(configuration);

        return services;
    }

    private static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
        );

        return services;
    }

}
