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
        Task AdicionarAsync(Pedido pedido);
        Task CancelarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
    }
}
