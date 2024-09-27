using Application.Commands;
using Application.Interfaces;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Handlers
{
    public class SepararPedidoCommandHandler : IRequestHandler<SepararPedidoCommand, bool>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly INotificacaoService _notificacaoService;

        public SepararPedidoCommandHandler(IPedidoRepository pedidoRepository, INotificacaoService notificacaoService)
        {
            _pedidoRepository = pedidoRepository;
            _notificacaoService = notificacaoService;
        }

        public async Task<bool> Handle(SepararPedidoCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync(command.CodigoPedido);

            if (pedido is null || pedido.Status != StatusPedido.PagamentoConcluido)
                return false;
            
            pedido.AlterarStatus(StatusPedido.SeparandoPedido);
            await _pedidoRepository.AtualizarAsync(pedido);

            foreach (var item in pedido.Itens)
            {
                var produto = item.Produto;

                if (produto.QuantidadeEmEstoque < item.Quantidade)
                {
                    pedido.AlterarStatus(StatusPedido.AguardandoEstoque);
                    await _notificacaoService.EnviarNotificacaoEstoqueInsuficienteAsync(pedido);
                    await _pedidoRepository.AtualizarAsync(pedido);
                    return false;
                }

                produto.AjustarEstoque(item.Quantidade);
            }

            pedido.AlterarStatus(StatusPedido.Concluido);
            await _notificacaoService.EnviarNotificacaoStatusPedidoAsync(pedido, "Pedido Concluído com sucesso.");
            await _pedidoRepository.AtualizarAsync(pedido);

            return true;
        }
    }

}
