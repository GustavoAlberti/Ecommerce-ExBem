using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DescontoRepository : IDescontoRepository
    {
        private readonly ECommerceDbContext _context;

        public DescontoRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Desconto>> ObterDescontosValidosAsync(IEnumerable<int> produtoIds)
        {
            return await _context.Descontos
                .Where(d => produtoIds.Contains(d.ProdutoId.Value)
                            && d.DataInicio <= DateTime.Now
                            && d.DataFim >= DateTime.Now)
                .ToListAsync();
        }
    }
}
