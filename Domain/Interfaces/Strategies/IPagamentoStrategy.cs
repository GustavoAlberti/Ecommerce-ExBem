using Domain.Entities;

namespace Application.Interfaces.PagamentoStrategy
{
    public interface IPagamentoStrategy
    {
        Task<bool> ProcessarPagamentoAsync(Pedido pedido, int? numeroParcelas = null);
    }
}
