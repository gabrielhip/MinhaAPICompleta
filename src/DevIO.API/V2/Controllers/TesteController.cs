using DevIO.API.Controllers;
using DevIO.Business.Interfaces;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DevIO.API.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        private readonly ILogger _logger;

        public TesteController(INotificador notificador, IUser appUser, ILogger<TesteController> logger) : base(notificador, appUser)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Valor()
        {
            throw new Exception("error");

            //testando o logging elmahio
            //try
            //{
            //    var i = 0;
            //    var result = 42 / i;
            //}
            //catch (DivideByZeroException e)
            //{
            //    //extension method do elmah, para que seja possível enviar o erro que aconteceu para o servidor do elmah
            //    //ao invés de lançar o erro, estou simplesmente logando o mesmo
            //    e.Ship(HttpContext);                
            //}


            //tipos de log possíveis:
            _logger.LogTrace("Log de Trace"); //log mais simples, de desenvolvimento
            _logger.LogDebug("Log de Debu"); //log usado para debug
            _logger.LogInformation("Log de Informação");
            _logger.LogWarning("Log de Aviso");
            _logger.LogError("Log de Erro");
            _logger.LogCritical("Log de Problema Critico");

            return "Sou a V2";
        }
    }
}
