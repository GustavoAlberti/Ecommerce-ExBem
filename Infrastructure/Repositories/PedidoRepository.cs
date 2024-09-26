using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ECommerceDbContext _context;

        public PedidoRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido); 
            await _context.SaveChangesAsync();
        }

        public async Task<Pedido> ObterPorCodigoPedidoAsync(string codigoPedido)
        {
            var pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.CodigoPedido == codigoPedido);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"Pedido com o código '{codigoPedido}' não foi encontrado.");
            }

            return pedido;

        }

        public async Task<Pedido> ObterPorCodigoPedidoAsync2(string codigoPedido, bool incluirPagamento = false)
        {
            var query = _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Include(p => p.Usuario)
                .AsQueryable();

            if (incluirPagamento)
                query = query.Include(p => p.Pagamento); // Incluir pagamento quando necessário.

            var pedido = await query.FirstOrDefaultAsync(p => p.CodigoPedido == codigoPedido);

            if (pedido == null)
            {
                throw new KeyNotFoundException($"Pedido com o código '{codigoPedido}' não foi encontrado.");
            }

            return pedido;
        }
    }
}
