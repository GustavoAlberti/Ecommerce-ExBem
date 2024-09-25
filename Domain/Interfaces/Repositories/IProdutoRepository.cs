using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> ObterPorCodigosAsync(IEnumerable<string> codigosProduto);

        Task AtualizarEstoqueAsync(IEnumerable<Produto> produtos);

        Task<IEnumerable<Produto>> ObterPorIdsAsync(IEnumerable<int> produtoIds);
    }
}
