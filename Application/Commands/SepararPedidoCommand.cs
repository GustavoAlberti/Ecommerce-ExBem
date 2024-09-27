using MediatR;

namespace Application.Commands
{
    public class SepararPedidoCommand : IRequest<bool>
    {
        public string CodigoPedido { get; }

        public SepararPedidoCommand(string codigoPedido)
        {
            CodigoPedido = codigoPedido;
        }
    }

}
