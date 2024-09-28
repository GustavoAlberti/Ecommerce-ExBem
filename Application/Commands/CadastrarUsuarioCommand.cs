using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
