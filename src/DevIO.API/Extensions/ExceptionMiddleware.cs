using System;
using System.Net;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace DevIO.API.Extensions
{
    //Qualquer Exception que ao chamar o Web API e ocorrer um erro não tratado, vai cair nesse middleware
    //E esse Middleware irá chamar o ShipAsync do ElmahIO e devolver o código de erro 500.
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            await exception.ShipAsync(context); //extension method, aonde eu passo o HttpContext 
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }
}
