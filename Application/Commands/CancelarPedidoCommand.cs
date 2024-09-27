using MediatR;

namespace Application.Commands
{
    public class CancelarPedidoCommand : IRequest<Unit>
    {
        public string CodigoPedido { get; }

        public CancelarPedidoCommand(string codigoPedido)
        {
            CodigoPedido = codigoPedido;
        }
    }
}
