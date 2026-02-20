using Microsoft.Extensions.DependencyInjection;
using Sincera.Claims.Domain.Interfaces;

namespace Sincera.Claims.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IFileInspector, FileInspector>();
        return services;
    }
}
