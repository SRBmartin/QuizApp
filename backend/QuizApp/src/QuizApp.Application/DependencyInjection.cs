using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using QuizApp.Application.Common.Behaviours;
using System.Reflection;

namespace QuizApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(executingAssembly);
        });

        services.AddValidatorsFromAssembly(executingAssembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(executingAssembly);
        }, Assembly.GetExecutingAssembly());

        return services;
    }

}
