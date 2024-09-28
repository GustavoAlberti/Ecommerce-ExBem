using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> ObterPorCodigosAsync(IEnumerable<string> codigosProduto);
        Task AtualizarEstoqueAsync(IEnumerable<Produto> produtos);
        Task<IEnumerable<Produto>> ObterPorIdsAsync(IEnumerable<int> produtoIds);
        Task AdicionarAsync(Produto produto);

    }
}
