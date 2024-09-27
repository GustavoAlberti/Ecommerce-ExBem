using Application.DTOs;
using MediatR;

namespace Application.Querys
{
    public class BuscarPedidoQuery : IRequest<PedidoResponseDto>
    {
        public string CodigoPedido { get; }

        public BuscarPedidoQuery(string codigoPedido)
        {
            CodigoPedido = codigoPedido;
        }
    }

}
