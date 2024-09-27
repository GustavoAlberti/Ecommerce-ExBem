using Domain.Entities.Enum;

namespace Domain.Entities
{
    public class Pagamento
    {
        public int Id { get; private set; }
        public int PedidoId { get; private set; }
        public TipoPagamento TipoPagamento { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataPagamento { get; private set; }
        public bool PagamentoConcluido { get; private set; }
        public int? NumeroParcelas { get; private set; }

        private Pagamento() { }

        public Pagamento(int pedidoId, TipoPagamento tipoPagamento, decimal valor, int? numeroParcelas = null)
        {
            PedidoId = pedidoId;
            TipoPagamento = tipoPagamento;
            Valor = valor;
            DataPagamento = DateTime.UtcNow;
            PagamentoConcluido = false;
            NumeroParcelas = (tipoPagamento == TipoPagamento.CartaoDeCredito) ? numeroParcelas : 1;
        }

        public bool ConcluirPagamento()
        {
            if (PagamentoConcluido)
                return false; 
            
            PagamentoConcluido = true;
            DataPagamento = DateTime.UtcNow; 
            return true;
        }

        public void CancelarPagamento()
        {
            PagamentoConcluido = false;
            DataPagamento = DateTime.MinValue;
        }
    }
}
