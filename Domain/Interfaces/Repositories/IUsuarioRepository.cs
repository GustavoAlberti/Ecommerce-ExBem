using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorLoginAsync(string usuarioLogin);
        Task<Usuario> ObterPorEmailAsync(string email);
        Task AdicionarAsync(Usuario usuario);
    }
}
