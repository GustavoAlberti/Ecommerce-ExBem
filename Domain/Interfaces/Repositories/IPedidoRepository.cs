using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> ObterPorCodigoPedidoAsync(string codigoPedido);

        Task<Pedido> ObterPorCodigoPedidoAsync2(string codigoPedido, bool incluirPagamento = false);
        Task AdicionarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
    }
}
