using QuizApp.Infrastructure;

namespace QuizApp.Api.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureServices(configuration);

        return services;
    }

}
