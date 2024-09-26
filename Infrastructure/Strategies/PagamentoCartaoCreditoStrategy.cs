using Application.Interfaces.PagamentoStrategy;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Strategies
{
    public class PagamentoCartaoCreditoStrategy : IPagamentoStrategy
    {
        private const int MaxTentativas = 3;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPedidoRepository _pedidoRepository;

        public PagamentoCartaoCreditoStrategy(IPagamentoRepository pagamentoRepository, IPedidoRepository pedidoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<bool> ProcessarPagamentoAsync(Pedido pedido, int? numeroParcelas = null)
        {
            var parcelas = numeroParcelas ?? 1;

            return await TentarProcessarPagamentoComRetentativaAsync(pedido, parcelas, 1);
        }

        private async Task<bool> TentarProcessarPagamentoComRetentativaAsync(Pedido pedido, int numeroParcelas, int tentativaAtual)
        {
            try
            {
                var valorParcela = pedido.ValorTotal / numeroParcelas;

                var pagamento = new Pagamento(pedido.Id, TipoPagamento.CartaoDeCredito, pedido.ValorTotal, numeroParcelas);
                pagamento.ConcluirPagamento();

                
                await _pagamentoRepository.AdicionarPagamentoAsync(pagamento);

                
                pedido.AlterarStatus(StatusPedido.PagamentoConcluido);
                await _pedidoRepository.AtualizarAsync(pedido);

                return true;
            }
            catch (Exception ex)
            {
                if (tentativaAtual >= MaxTentativas)
                    return false;
                

                await Task.Delay(TimeSpan.FromSeconds(2)); // Espera antes da próxima tentativa

                return await TentarProcessarPagamentoComRetentativaAsync(pedido, numeroParcelas, tentativaAtual + 1);
            }
        }
    }
}
