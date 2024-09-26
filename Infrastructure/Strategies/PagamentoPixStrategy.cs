using Application.Interfaces.PagamentoStrategy;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Strategies
{
    public class PagamentoPixStrategy : IPagamentoStrategy
    {
        private const decimal DescontoPix = 0.05m; 
        private const int MaxTentativas = 3;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IPedidoRepository _pedidoRepository;

        public PagamentoPixStrategy(IPagamentoRepository pagamentoRepository, IPedidoRepository pedidoRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _pedidoRepository = pedidoRepository;
        }

        public async Task<bool> ProcessarPagamentoAsync(Pedido pedido, int? numeroParcelas = null)
        {
            var valorDesconto = pedido.ValorTotal * DescontoPix;
            pedido.AplicarDesconto(valorDesconto);

            return await TentarProcessarPagamentoComRetentativaAsync(pedido, 1);
        }

        private async Task<bool> TentarProcessarPagamentoComRetentativaAsync(Pedido pedido, int tentativaAtual)
        {
            try
            {
                var pagamento = new Pagamento(pedido.Id, TipoPagamento.Pix, pedido.ValorTotal, 1);

                pagamento.ConcluirPagamento();

                await _pagamentoRepository.AdicionarPagamentoAsync(pagamento);

                pedido.AlterarStatus(StatusPedido.PagamentoConcluido);
                await _pedidoRepository.AtualizarAsync(pedido);

                return true;
            }
            catch (Exception)
            {
                if (tentativaAtual >= MaxTentativas)
                    return false;
                
                await Task.Delay(TimeSpan.FromSeconds(2));

                return await TentarProcessarPagamentoComRetentativaAsync(pedido, tentativaAtual + 1);
            }
        }
    }

}
