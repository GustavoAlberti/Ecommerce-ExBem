using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum StatusPedido
    {
        AguardandoPagamento =1,
        ProcessandoPagamento = 2,
        PagamentoConcluido = 3,
        SeparandoPedido = 4,
        Concluido = 5,
        AguardandoEstoque = 6,
        Cancelado = 7
    }
}
