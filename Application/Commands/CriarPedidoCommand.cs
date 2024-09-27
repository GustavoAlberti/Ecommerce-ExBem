using Application.DTOs;
using MediatR;

namespace Application.Commands
{
    public class CriarPedidoCommand : IRequest<PedidoResponseDto>
    {
        public string UsuarioLogin { get; }
        public IEnumerable<CriarItemPedidoCommand> Itens { get; }

        public CriarPedidoCommand(string usuarioLogin, IEnumerable<CriarItemPedidoCommand> itens)
        {
            UsuarioLogin = usuarioLogin;
            Itens = itens;
        }
    }

    public class CriarItemPedidoCommand
    {
        public string CodigoProduto { get; }
        public int Quantidade { get; }

        public CriarItemPedidoCommand(string codigoProduto, int quantidade)
        {
            CodigoProduto = codigoProduto;
            Quantidade = quantidade;
        }
    }

}
