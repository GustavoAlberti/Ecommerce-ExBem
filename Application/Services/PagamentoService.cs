using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.PagamentoStrategy;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;


namespace Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IDictionary<TipoPagamento, IPagamentoStrategy> _strategies;
        private readonly IPedidoRepository _pedidoRepository;

        public PagamentoService(IDictionary<TipoPagamento, IPagamentoStrategy> strategies, IPedidoRepository pedidoRepository)
        {
            _strategies = strategies;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<PagamentoResponseDto> ProcessarPagamentoAsync(PagarPedidoDto pagamentoDto)
        {
            var pedido = await _pedidoRepository.ObterPorCodigoPedidoAsync2(pagamentoDto.CodigoPedido);

            // Verifica se o pagamento já foi concluído
            if (pedido.Status == StatusPedido.PagamentoConcluido)
            {
                return new PagamentoResponseDto(
                    "O pagamento já foi realizado anteriormente.",
                    pedido.Itens.Sum(i => i.Preco * i.Quantidade),
                    pedido.Pagamento.Valor,
                    pedido.Pagamento.NumeroParcelas,
                    pedido.Pagamento.TipoPagamento.ToString(),
                    "Pagamento Concluído"
                );
            }

            pedido.AlterarStatus(StatusPedido.ProcessandoPagamento);
            await _pedidoRepository.AtualizarAsync(pedido);


            if (!_strategies.TryGetValue(pagamentoDto.TipoPagamento, out var strategy) || strategy is null)
            {
                return new PagamentoResponseDto(
                        "Tipo de pagamento inválido.",
                        0,
                        0,
                        pagamentoDto.NumeroParcelas,
                        pagamentoDto.TipoPagamento.ToString(),
                        "Falha"
                    );
            }

            var pagamentoConcluido = await strategy.ProcessarPagamentoAsync(pedido, pagamentoDto.NumeroParcelas);

            if (pagamentoConcluido)
            {

                return new PagamentoResponseDto(
                        "Pagamento realizado com sucesso.",
                        pedido.Itens.Sum(i => i.Preco * i.Quantidade),
                        pedido.Pagamento.TipoPagamento == TipoPagamento.CartaoDeCredito ? null : pedido.Pagamento.Valor,
                        pedido.Pagamento.TipoPagamento == TipoPagamento.CartaoDeCredito ? pedido.Pagamento.NumeroParcelas : null,
                        pedido.Pagamento.TipoPagamento.ToString(),
                        "Pagamento Concluído"
                    );
            }

            // Atualizar estado do pedido para "Cancelado" (caso o pagamento não seja concluído)
            pedido.AlterarStatus(StatusPedido.Cancelado);
            await _pedidoRepository.AtualizarAsync(pedido);

            // Retornar o DTO de falha
            return new PagamentoResponseDto(
                "Falha no processamento do pagamento.",
                pedido.Pagamento.Valor,
                pedido.Pagamento.Valor,
                pedido.Pagamento.NumeroParcelas,
                pedido.Pagamento.TipoPagamento.ToString(),
                "Cancelado"
            );
        }
    }
}