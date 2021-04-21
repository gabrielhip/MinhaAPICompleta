using DevIO.Business.Interfaces;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
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
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            //services
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IProdutoService, ProdutoService>();

            //notificador
            services.AddScoped<INotificador, Notificador>();

            return services;
        }
    }
}