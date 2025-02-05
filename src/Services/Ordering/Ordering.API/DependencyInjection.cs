namespace Ordering.API;

using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Security;
using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarter();

        var connectionName = Security.GetConnectionString(configuration.GetConnectionString("Database")!);
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks()
                .AddSqlServer(connectionName);

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();
        app.UseExceptionHandler(options => { });
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        return app;
    }
}
