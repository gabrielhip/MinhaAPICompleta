using AutoMapper;
using DevIO.API.Configuration;
using DevIO.API.Extensions;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //Configurando o contexto
            services.AddDbContext<MeuDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentityConfiguration(Configuration); //adicionando a configuração do Identity

            services.AddAutoMapper(typeof(Startup)); //Adicionando o automapper passando o tipo Startup

            services.WebApiConfig(); //configuração de MVC e CORS

            services.AddSwaggerConfig(); //configuração do Swagger

            services.AddLoggingConfiguration(Configuration); //configuração de logging elmahio

            services.ResolveDependencies(); //Configurando a injeção de dependência
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("Development"); //usando a configuração de CORS criada, através da policy "Development"
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCors("Production"); //usando a configuração de CORS criada, através da policy "Production"
                app.UseHsts(); //recurso de segurança com um header (chave/valor) que passa da aplicação pro client, e o client vai entender que a aplicação só conversa em https
            }

            app.UseAuthentication(); //precisa sempre vir antes da configuração do MVC

            app.UseMiddleware<ExceptionMiddleware>(); //usando o middleware configurado para logar todas as exceptions no elmahio

            app.UseMvcConfiguration();

            app.UseSwaggerConfig(provider); //usa a configuração do Swagger

            app.UseLoggingConfiguration(); //usa a configuração do logging elmahio
        }
    }
}
