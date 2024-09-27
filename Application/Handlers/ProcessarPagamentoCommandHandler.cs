using Application.Commands;
using Application.DTOs;
using Application.Interfaces.PagamentoStrategy;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Handlers
{
    public class ProcessarPagamentoCommandHandler : IRequestHandler<ProcessarPagamentoCommand, PagamentoResponseDto>
    {
        private readonly IDictionary<TipoPagamento, IPagamentoStrategy> _strategies;
        private readonly IPedidoRepository _pedidoRepository;

        public ProcessarPagamentoCommandHandler(
            IDictionary<TipoPagamento, IPagamentoStrategy> strategies,
            IPedidoRepository pedidoRepository)
        {
            _strategies = strategies;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PagamentoResponseDto> Handle(ProcessarPagamentoCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync(command.CodigoPedido);

            if (pedido is null)
                return new PagamentoResponseDto("Pedido não encontrado.", 0, 0, null, command.TipoPagamento.ToString(), "Falha");
            
            if (pedido.Status == StatusPedido.PagamentoConcluido)
            {
                return new PagamentoResponseDto(
                    "O pagamento já foi realizado.",
                    pedido.Itens.Sum(i => i.Preco * i.Quantidade),
                    pedido.Pagamento?.Valor ?? 0,
                    pedido.Pagamento?.NumeroParcelas,
                    pedido.Pagamento?.TipoPagamento.ToString() ?? command.TipoPagamento.ToString(),
                    "Pagamento Concluído"
                );
            }

            pedido.AlterarStatus(StatusPedido.ProcessandoPagamento);
            await _pedidoRepository.AtualizarAsync(pedido);

            if (!_strategies.TryGetValue(command.TipoPagamento, out var strategy) || strategy is null)
            {
                return new PagamentoResponseDto(
                    "Tipo de pagamento inválido.",
                    0,
                    0,
                    command.NumeroParcelas,
                    command.TipoPagamento.ToString(),
                    "Falha"
                );
            }

            var pagamentoConcluido = await strategy.ProcessarPagamentoAsync(pedido, command.NumeroParcelas);

            if (pagamentoConcluido)
            {
                return new PagamentoResponseDto(
                    "Pagamento realizado com sucesso.",
                    pedido.Itens.Sum(i => i.Preco * i.Quantidade),
                    pedido.Pagamento?.Valor ?? 0,
                    pedido.Pagamento?.NumeroParcelas,
                    command.TipoPagamento.ToString(),
                    "Pagamento Concluído"
                );
            }

            
            pedido.AlterarStatus(StatusPedido.Cancelado);
            await _pedidoRepository.AtualizarAsync(pedido);

            return new PagamentoResponseDto("Falha no processamento do pagamento.", pedido.Pagamento?.Valor ?? 0, 0, command.NumeroParcelas, command.TipoPagamento.ToString(), "Cancelado");
        }
    }

}
