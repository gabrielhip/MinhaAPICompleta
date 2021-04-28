using System.Text;
using DevIO.API.Data;
using DevIO.Api.Extensions;
using DevIO.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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

            
            // JWT
            var appSettingsSection = configuration.GetSection("AppSettings"); //busca a seção "AppSettings" no appsettings.json
            services.Configure<AppSettings>(appSettingsSection); //configurando no ASP.NET Core que a classe AppSettings representa a seção buscada no appsettings.json

            var appSettings = appSettingsSection.Get<AppSettings>(); //pega os dados da classe AppSettings
            var key = Encoding.ASCII.GetBytes(appSettings.Secret); //define uma chave em ASCII através da variável Secret

            services.AddAuthentication(x => //adiciona uma autenticação
            {
                x.DefaultAuthenticateScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme; //toda vez que for autenticar alguém, o padrão de autenticação é pra gerar o token
                x.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme; //verificar se toda requisição que chegar está autenticada
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; //se tem garantias que está trabalhando somente com https, setar true
                x.SaveToken = true; //se o token deve ser guardado no HttpAuthenticationProperties,
                x.TokenValidationParameters = new TokenValidationParameters //cria uma série de novos parâmetros
                {
                    ValidateIssuerSigningKey = true, //valida se quem está emitindo é o mesmo quando receber o token
                    IssuerSigningKey = new SymmetricSecurityKey(key), //transforma a chave em uma chave criptografada
                    ValidateIssuer = true, //valida apenas o issuer conforme o nome
                    ValidateAudience = true, //valida em qual audiencia seu token é valido
                    ValidAudience = appSettings.ValidoEm, //seta a audiência
                    ValidIssuer = appSettings.Emissor //seta o emissor
                };
            });

            return services;
        }
    }
}