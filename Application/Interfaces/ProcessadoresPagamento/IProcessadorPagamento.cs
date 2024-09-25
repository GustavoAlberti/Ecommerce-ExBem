using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ProcessadoresPagamento
{
    public interface IProcessadorPagamento
    {
        Task ProcessarPagamento(Pedido pedido, int? numeroParcelas = null);
    }
}
