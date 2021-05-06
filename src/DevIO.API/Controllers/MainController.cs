using DevIO.Business.Interfaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace DevIO.API.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase //abstrata = só pode ser herdada
    {
        private readonly INotificador _notificador;
        public readonly IUser AppUser;

        protected Guid UsuarioId { get; set; }
        public bool UsuarioAutenticado { get; set; }

        protected MainController(INotificador notificador, 
                                 IUser appUser)
        {
            _notificador = notificador;
            AppUser = appUser;

            //disponibiliza propriedades já setadas para todas as outras controllers
            if (appUser.IsAuthenticated())
            {
                UsuarioId = appUser.GetUserId();
                UsuarioAutenticado = true;
            }
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        //CustomResponse que valida se existe algum tipo de operação independente de onde ela surgiu e retorna um Ok
        //caso tenha, eu retorno uma lista de mensagens para o cliente
        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes()
                    .Select(n => n.Mensagem) //criando uma lista de mensagens dentro do errors
            });
        }

        //trabalhar os erros recebidos na ModelState e chamar o próximo método de CustomResponse
        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            //se for inválida vou obter os erros e depois chamar o próximo CustomResponse
            if (!modelState.IsValid) 
                NotificarErroModelInvalida(modelState);

            return CustomResponse();
        }

        //recebe o ModelState também e adiciona a mensagem de erro na pilha de notificações
        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            //obtem a coleção de erros da modelstate
            var erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }

        //lança o objeto de notificação para a fila de erros
        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}
