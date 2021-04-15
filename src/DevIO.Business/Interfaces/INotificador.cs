using System.Collections.Generic;
using DevIO.Business.Notificacoes;

namespace DevIO.Business.Interfaces
{
    public interface INotificador
    {
        bool TemNotificacao(); //validar se tem notificação
        List<Notificacao> ObterNotificacoes(); //obter notificações
        void Handle(Notificacao notificacao); //manipular uma notificação quando ela for lançada
    }
}