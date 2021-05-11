using DevIO.Api.Extensions;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevIO.API.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //configurando chaves do logger elmah.io
            services.AddElmahIo(o =>
            {
                o.ApiKey = "7e07da8a0d91448d96363bd1ae2d2b6f";
                o.LogId = new Guid("156d0aea-a3b8-41da-8775-d13d2e0a658d");
            });

            //conectando o elmah com o logger, caso queira logar coisas além
            //services.AddLogging(builder =>
            //{
            //    //configuração para o elmah logar também os logs do asp.net core, e não apenas as situações em que ocorrer exception
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "7e07da8a0d91448d96363bd1ae2d2b6f";
            //        o.LogId = new Guid("156d0aea-a3b8-41da-8775-d13d2e0a658d");
            //    });
            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning); //logando warning pra pior (error, critical)
            //});

            //configuração de health checks
            services.AddHealthChecks()
                .AddElmahIoPublisher("7e07da8a0d91448d96363bd1ae2d2b6f", new Guid("156d0aea-a3b8-41da-8775-d13d2e0a658d"), "API Fornecedores") //configurando o Elmah como Publisher dos health checks
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection"))) //adiciona o check criado nas extensions
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQl"); //verifica se o banco da aplicação está funcionando

            //configuração de interface para health checks, onde auxilia a ter um pouco mais de informações
            services.AddHealthChecksUI();

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            //utilizando elmah.io
            app.UseElmahIo();

            //usa a configuração de health checks
            app.UseHealthChecks("/api/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //usa a configuração de interface para o health checks
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/api/hc-ui";
            });

            return app;
        }
    }
}
