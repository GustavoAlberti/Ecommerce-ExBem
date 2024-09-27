using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IPagamentoRepository
    {
        Task AdicionarPagamentoAsync(Pagamento pagamento);
        Task<Pagamento> ObterPorPedidoIdAsync(int pedidoId);
        Task AtualizarPagamentoAsync(Pagamento pagamento);
    }
}
