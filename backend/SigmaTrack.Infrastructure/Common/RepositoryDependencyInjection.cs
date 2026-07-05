using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace SigmaTrack.Infrastructure.Common;

public static class RepositoryDependencyInjection
{
    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services, Assembly assembly)
    {
        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"));

        foreach (var implementationType in repositoryTypes)
        {
            var interfaceType = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementationType.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }
        return services;
    }
}