﻿using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorLoginAsync(string usuarioLogin);
    }
}
