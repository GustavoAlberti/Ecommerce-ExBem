﻿using Domain.Entities;
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

        public async Task<Usuario> ObterPorLoginAsync(string usuarioLogin)
        {
            return await _context.Usuarios
                .Where(u => u.UsuarioLogin == usuarioLogin)
                .FirstOrDefaultAsync();

        }
    }

}
