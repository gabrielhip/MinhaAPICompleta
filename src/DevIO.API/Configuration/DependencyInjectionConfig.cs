using DevIO.Business.Interfaces;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            //contexto
            services.AddScoped<MeuDbContext>();

            //repositorios
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();

            return services;
        }
    }
}