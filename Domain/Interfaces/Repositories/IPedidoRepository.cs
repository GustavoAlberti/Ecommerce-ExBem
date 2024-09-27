using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> ObterPorCodigoPedidoAsync(string codigoPedido, bool incluirPagamento = false);
        Task AdicionarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
    }
}
