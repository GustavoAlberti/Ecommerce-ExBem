using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IPagamentoRepository
    {
        Task AdicionarPagamentoAsync(Pagamento pagamento);
        Task<Pagamento> ObterPorPedidoIdAsync(int pedidoId);
        Task AtualizarPagamentoAsync(Pagamento pagamento);
    }
}
