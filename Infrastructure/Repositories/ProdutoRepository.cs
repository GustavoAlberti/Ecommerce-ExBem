using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ECommerceDbContext _context;

        public ProdutoRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Produto>> ObterPorCodigosAsync(IEnumerable<string> codigosProduto)
        {
            return await _context.Produtos
                .Where(p => codigosProduto.Contains(p.CodigoProduto))
                .ToListAsync(); 
        }

        public async Task AtualizarEstoqueAsync(IEnumerable<Produto> produtos)
        {
            _context.Produtos.UpdateRange(produtos);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Produto>> ObterPorIdsAsync(IEnumerable<int> produtoIds)
        {
            return await _context.Produtos
                .Where(p => produtoIds.Contains(p.Id))
                .ToListAsync(); 
        }

        public async Task AdicionarAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }
    }
}
