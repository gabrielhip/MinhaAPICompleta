using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DevIO.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //suprimindo a forma da validação da view model automática, pois quero validar eu mesmo para personalizar as respostas para o usuário
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            //configurando CORS
            //abrindo a aplicação para quem quiser acessar criando a policy "Development"
            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            //se alguém chamar a aplicação via HTTP, automaticamente o .net core faz o redirecionamento interno para HTTPS
            app.UseHttpsRedirection();

            //usando a configuração de CORS criada, através da policy "Development"
            app.UseCors("Development");

            app.UseMvc();

            return app;
        }
    }
}