using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ECommerceDbContext _context;

        public UsuarioRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            return usuario;
        }

        public async Task<Usuario> ObterPorLoginAsync(string usuarioLogin)
        {
            var usuario = await _context.Usuarios
             .Where(u => u.UsuarioLogin == usuarioLogin)
             .FirstOrDefaultAsync();

            return usuario;
        }
    }

}
