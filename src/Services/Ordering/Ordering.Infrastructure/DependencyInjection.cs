using BuildingBlocks.Security;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data.Interceptors;

namespace Ordering.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Security.GetConnectionString(
    configuration.GetConnectionString("Database")!);


        // Add services to the container.
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((c, options) =>
        {
            options.AddInterceptors(c.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });

        // services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }
}
