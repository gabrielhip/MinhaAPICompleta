using System.Collections.Generic;
using System.Linq;
using DevIO.Business.Interfaces;

namespace DevIO.Business.Notificacoes
{
    public class Notificador : INotificador
    {
        private List<Notificacao> _notificacoes;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        //adiciona a notificação lançada dentro da lista de notificações
        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        //retorna a lista de notificações
        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        //verifica se existe uma notificação
        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}