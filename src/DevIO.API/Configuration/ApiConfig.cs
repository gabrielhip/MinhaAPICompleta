using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace DevIO.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection WebApiConfig(this IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //adiciona as configurações de versionamento da API
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; //assume uma versão default quando não tiver nenhuma versão especificada
                options.DefaultApiVersion = new ApiVersion(1, 0); //versão default = 1.0
                options.ReportApiVersions = true; //reporta no header do response se a api está obsoleta
            });

            services.AddVersionedApiExplorer(options =>
            {
                //padrão que irá agrupar a versão da API
                //no caso irá ficar: v + número equivalente a versão
                //ex: v1, v2, v3
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true; //se tiver uma rota padrão, ele substitui o número da versão pela versão default se não possuir a mesma
            });

            //suprimindo a forma da validação da view model automática, pois quero validar eu mesmo para personalizar as respostas para o usuário
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            //configurando CORS
            //o CORS serve para "relaxar um pouco" a forma de outras origens acessarem a sua aplicação
            //somente o navegador implementa o CORS, o POSTMAN não
            //abrindo a aplicação para quem quiser acessar criando a policy "Development"
            //restringindo a aplicação com determinadas regras criando a policy "Production"
            services.AddCors(options =>
            {
                options.AddPolicy("Development",
                    builder => 
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());

                //options.AddDefaultPolicy( //adicionando política padrão caso não esteja utilizando a política Development ou Production
                //    builder =>
                //        builder
                //            .AllowAnyOrigin()
                //            .AllowAnyMethod()
                //            .AllowAnyHeader()
                //            .AllowCredentials());

                options.AddPolicy("Production",
                    builder =>
                    builder
                        .WithMethods("GET") //só vou permitir métodos com verbo GET
                        .WithOrigins("http://desenvolvedor.io") //apenas para a origem desse site
                        .SetIsOriginAllowedToAllowWildcardSubdomains() //se outra aplicação estiver rodando no mesmo subdomínio da minha API, eu permito
                        //.WithHeaders(HeaderNames.ContentType, "x-custom-header") //define o tipo de header que a aplicação vai aceitar
                        .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseMvcConfiguration(this IApplicationBuilder app)
        {
            //se alguém chamar a aplicação via HTTP, automaticamente o .net core faz o redirecionamento interno para HTTPS
            app.UseHttpsRedirection();            

            app.UseMvc();

            return app;
        }
    }
}