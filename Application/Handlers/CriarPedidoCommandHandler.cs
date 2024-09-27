using Application.Commands;
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Handlers
{
    public class CriarPedidoCommandHandler : IRequestHandler<CriarPedidoCommand, PedidoResponseDto>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IDescontoRepository _descontoRepository;

        public CriarPedidoCommandHandler(IPedidoRepository pedidoRepository, IUsuarioRepository usuarioRepository,
                                         IProdutoRepository produtoRepository, IDescontoRepository descontoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoRepository = produtoRepository;
            _descontoRepository = descontoRepository;
        }

        public async Task<PedidoResponseDto> Handle(CriarPedidoCommand command, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObterPorLoginAsync(command.UsuarioLogin) ?? throw new KeyNotFoundException("Usuário não encontrado.");

            var pedido = new Pedido(usuario.Id);

            var produtos = await _produtoRepository.ObterPorCodigosAsync(command.Itens.Select(i => i.CodigoProduto));

            if (produtos.Count() != command.Itens.Count())
                throw new KeyNotFoundException("Alguns produtos não foram encontrados.");
            

            foreach (var item in command.Itens)
            {
                var produto = produtos.First(p => p.CodigoProduto == item.CodigoProduto);
                pedido.AdicionarItem(new ItemPedido(produto, item.Quantidade, produto.Preco));
            }

            var descontos = await _descontoRepository.ObterDescontosValidosAsync(pedido.Itens.Select(i => i.ProdutoId));

            descontos.ForEach(pedido.AplicarDescontoParaProduto);

            pedido.AtualizarValorTotal();
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            await _pedidoRepository.AdicionarAsync(pedido);

            return new PedidoResponseDto(
                codigoPedido: pedido.CodigoPedido,
                usuarioNome: usuario.Nome,
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
