using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Handlers
{
    public class CancelarPedidoCommandHandler : IRequestHandler<CancelarPedidoCommand, Unit>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly INotificacaoService _notificacaoService;

        public CancelarPedidoCommandHandler(IPedidoRepository pedidoRepository,
                                            IPagamentoRepository pagamentoRepository,
                                            INotificacaoService notificacaoService)
        {
            _pedidoRepository = pedidoRepository;
            _pagamentoRepository = pagamentoRepository;
            _notificacaoService = notificacaoService;
        }

        public async Task<Unit> Handle(CancelarPedidoCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync(command.CodigoPedido, incluirPagamento: true);

            VerificarPedido(pedido);
            await VerificarStatusPedidoParaCancelamento(pedido);

            pedido.AlterarStatus(StatusPedido.Cancelado);
            
            await Task.WhenAll(
                _pedidoRepository.AtualizarAsync(pedido),
                _notificacaoService.EnviarNotificacaoStatusPedidoAsync(pedido, "Seu pedido foi cancelado e o pagamento estornado.")
            );

            return Unit.Value;
        }

        
        private void VerificarPedido(Pedido pedido)
        {
            if (pedido == null)
                throw new KeyNotFoundException("Pedido não encontrado.");
        }

        private async Task VerificarStatusPedidoParaCancelamento(Pedido pedido)
        {
            if (pedido.Status == StatusPedido.Cancelado)
                throw new InvalidOperationException("O pedido já está cancelado.");

            if (pedido.Status != StatusPedido.AguardandoPagamento && pedido.Status != StatusPedido.AguardandoEstoque)
                throw new InvalidOperationException("O pedido não pode ser cancelado nesse status.");

            if (pedido.Status == StatusPedido.AguardandoEstoque)
                await CancelarPagamento(pedido.Pagamento);
            
        }

        private async Task CancelarPagamento(Pagamento pagamento)
        {
            pagamento.CancelarPagamento();
            await _pagamentoRepository.AtualizarPagamentoAsync(pagamento);
        }

    }


}
