using Microsoft.Extensions.DependencyInjection;
using Sincera.Claims.Domain.Interfaces;

namespace Sincera.Claims.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDocumentValidationService, DocumentValidationService>();
        return services;
    }
}
