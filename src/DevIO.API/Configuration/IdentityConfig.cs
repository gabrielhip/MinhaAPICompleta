using DevIO.API.Data;
using DevIO.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>() //adicionando identidade padrão
                .AddRoles<IdentityRole>() //adicionando roles padrões
                .AddEntityFrameworkStores<ApplicationDbContext>() //indica que estou trabalhando com entityframework
                .AddErrorDescriber<IdentityMensagensPortugues>() //adiciona a configuração de customização de mensagens para português
                .AddDefaultTokenProviders(); //adiciona recursos para poder gerar tokens e realizar autenticação

            return services;
        }
    }
}