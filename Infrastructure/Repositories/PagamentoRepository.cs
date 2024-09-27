using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly ECommerceDbContext _context;

        public PagamentoRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarPagamentoAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task<Pagamento> ObterPorPedidoIdAsync(int pedidoId)
        {
            return await _context.Pagamentos.FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
        }

        public async Task AtualizarPagamentoAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();
        }

    }
}
