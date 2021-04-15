using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevIO.Business.Models
{
    public class Produto : Entity
    {
        public Guid FornecedorId { get; set; } // SEMPRE DECLARAR O ID E O EF RELATION DA MODEL/OBJETO QUE TIVER RELAÇÃO
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }        
        public decimal Valor { get; set; }        
        public DateTime DataCadastro { get; set; }
        public bool Ativo { get; set; }

        /* EF Relation */
        public Fornecedor Fornecedor { get; set; } // 1 Produto tem 1 Fornecedor // SEMPRE DECLARAR O ID E O EF RELATION DA MODEL/OBJETO QUE TIVER RELAÇÃO
    }
}
