using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IDescontoRepository
    {
        Task<List<Desconto>> ObterDescontosValidosAsync(IEnumerable<int> produtoIds);
    }
}
