using Application.DTOs;
using Application.Querys;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.QueryHandlers
{
    public class BuscarPedidoQueryHandler : IRequestHandler<BuscarPedidoQuery, PedidoResponseDto>
    {
        private readonly IPedidoRepository _pedidoRepository;

        public BuscarPedidoQueryHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PedidoResponseDto> Handle(BuscarPedidoQuery query, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync(query.CodigoPedido);

            if (pedido is null)
                throw new KeyNotFoundException("Pedido não encontrado.");
            
            return new PedidoResponseDto(
                codigoPedido: pedido.CodigoPedido,
                usuarioNome: pedido.Usuario.Nome,
                itens: pedido.Itens.Select(i => new ItemPedidoResponseDto(
                    i.Produto.CodigoProduto,
                    i.Preco,
                    i.Quantidade
                )).ToList(),
                pedido.ValorTotal,
                pedido.Status.ToString(),
                pedido.DataCriacao.ToString("yyyy-MM-dd")
            );
        }
    }

}
