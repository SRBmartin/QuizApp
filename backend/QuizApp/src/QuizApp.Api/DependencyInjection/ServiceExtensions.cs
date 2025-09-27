using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using QuizApp.Application;
using QuizApp.Infrastructure;
using Serilog;

namespace QuizApp.Api.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilogLogger(configuration);

        services.AddInfrastructureServices(configuration);

        services.AddApplicationServices();

        services.AddControllers();

        services.AddSwagger();

        services.AddCors(configuration);

        return services;
    }

    private static IServiceCollection AddSerilogLogger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(configuration)
        );

        return services;
    }

    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "QuizApp API",
                Version = "v1",
                Description = "QuizApp Restful API"
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter: Bearer {your JWT}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            o.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            };
            o.AddSecurityRequirement(securityRequirement);
        });

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Cors.Name, builder =>
            {
                var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
                if (origins is { Length: > 0 })
                {
                    builder.WithOrigins(origins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                }
                else
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }

                builder.SetPreflightMaxAge(TimeSpan.FromHours(1));
            });
        });

        return services;
    }

}
