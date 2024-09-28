using Application.Commands;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Handlers
{
    public class CadastrarUsuarioCommandHandler : IRequestHandler<CadastrarUsuarioCommand, bool>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public CadastrarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<bool> Handle(CadastrarUsuarioCommand command, CancellationToken cancellationToken)
        {
            var usuarioExistente = await _usuarioRepository.ObterPorLoginAsync(command.UsuarioLogin);

            if (usuarioExistente is not null)
                throw new InvalidOperationException("Já existe um usuário com o mesmo login.");
            
            var emailExistente = await _usuarioRepository.ObterPorEmailAsync(command.Email);

            if (emailExistente is not null)
                throw new InvalidOperationException("Já existe um usuário com o mesmo e-mail.");
            
            var usuario = new Usuario(command.Nome, command.Email, command.UsuarioLogin);
            await _usuarioRepository.AdicionarAsync(usuario);

            return true; 
        }
    }
}
