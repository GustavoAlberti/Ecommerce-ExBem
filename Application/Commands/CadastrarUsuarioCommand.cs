using MediatR;

namespace Application.Commands
{
    public class CadastrarUsuarioCommand : IRequest<bool>
    {
        public string Nome { get; }
        public string UsuarioLogin { get; }
        public string Email { get; }

        public CadastrarUsuarioCommand(string nome, string usuarioLogin, string email)
        {
            Nome = nome;
            UsuarioLogin = usuarioLogin;
            Email = email;
        }
    }
}
