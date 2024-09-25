using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.PagamentoStrategy
{
    public interface IPagamentoStrategy
    {
        Task<bool> ProcessarPagamentoAsync(Pedido pedido, int? numeroParcelas = null);
    }
}
