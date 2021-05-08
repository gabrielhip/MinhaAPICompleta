using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DevIO.API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
        {
            //cria a configuração do swagger generator
            services.AddSwaggerGen(c =>
            {
                //adiciona a documentação padrão
                c.OperationFilter<SwaggerDefaultValues>();

                //passando uma coleção de dados de segurança
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    //tipo do token + dado (que pode ser uma coleção de string)
                    {"Bearer", new string[] { }}
                };

                //configurando a segurança do swagger
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Insira o token JWT desta maneira: Bearer {seu token}",
                    Name = "Authorization", //nome do tipo do dado que irei passar
                    In = "header", //onde irei passar essa informação
                    Type = "apiKey" //tipo de dado que irei passar no header
                });

                c.AddSecurityRequirement(security);
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger(); //utiliza o swagger

            //onde irá montar a tela para visualizar a API
            app.UseSwaggerUI(options =>
            {
                //dá um foreach nas versões que eu tenho e gera um endpoint pra cada versão
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    //gera 1 json pra cada versão da API
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }
    }

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider; //interface do pacote de versionamento

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            //pega todas as versões da minha API e adiciona um doc pra cada uma delas
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        //criando uma documentação mínima para a API
        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = "API - desenvolvedor.io",
                Version = description.ApiVersion.ToString(),
                Description = "Esta API faz parte do curso REST com ASP.NET Core WebAPI.",
                Contact = new Contact() { Name = "Eduardo Pires", Email = "contato@desenvolvedor.io" },
                TermsOfService = "https://opensource.org/licenses/MIT",
                License = new License() { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
            };

            //se a versão está obsoleta, adiciona uma descrição extra
            if (description.IsDeprecated)
            {
                //concatena a descrição com essa informação abaixo
                info.Description += " Esta versão está obsoleta!";
            }

            return info;
        }
    }

    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated = apiDescription.IsDeprecated();

            if (operation.Parameters == null)
            {
                return;
            }
            
            //faz um foreach dentro de cada parâmetro para obter a descrição, etc
            //pois está trabalhando com várias versões
            foreach (var parameter in operation.Parameters.OfType<NonBodyParameter>())
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Default == null)
                {
                    parameter.Default = description.DefaultValue;
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}