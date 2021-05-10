using Elmah.Io.AspNetCore;
using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DevIO.API.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfiguration(this IServiceCollection services)
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

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            //utilizando elmah.io
            app.UseElmahIo();

            return app;
        }
    }
}
