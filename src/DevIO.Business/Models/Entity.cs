using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Business.Models
{
    public abstract class Entity //não pode ser instanciada, precisa ser herdada
    {
        public Guid Id { get; set; }

        protected Entity()
        {
            Id = new Guid();
        }
    }
}
