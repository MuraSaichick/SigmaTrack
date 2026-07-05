using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SigmaTrack.Application.Common
{
    public static class UseCaseDependencyInjection
    {
        public static IServiceCollection AddApplicationUseCases(this IServiceCollection services, Assembly assembly)
        {
            services.AddValidatorsFromAssembly(assembly);
            var applicationServices = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                           (t.Name.EndsWith("UseCase") || t.Name.EndsWith("Validator")));

            foreach (var implementationType in applicationServices)
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
}