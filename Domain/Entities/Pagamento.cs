using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Pagamento
    {
        public int Id { get; private set; }
        public int PedidoId { get; private set; }
        public string TipoPagamento { get; private set; } // e.g., "Pix", "Cartão de Crédito"
        public decimal Valor { get; private set; }
        public DateTime DataPagamento { get; private set; }
        public bool PagamentoConcluido { get; private set; }
        public int? NumeroParcelas { get; private set; } // null se não for parcelado

        private Pagamento() { }

        // Construtor para inicialização de um pagamento
        public Pagamento(int pedidoId, string tipoPagamento, decimal valor, int? numeroParcelas = null)
        {
            PedidoId = pedidoId;
            TipoPagamento = tipoPagamento ?? throw new ArgumentNullException(nameof(tipoPagamento));
            Valor = valor;
            DataPagamento = DateTime.UtcNow;
            PagamentoConcluido = false;
            NumeroParcelas = (tipoPagamento == "Cartão de Crédito") ? numeroParcelas : 1;
        }

        public bool ConcluirPagamento()
        {
            if (PagamentoConcluido)
                return false; 
            
            PagamentoConcluido = true;
            DataPagamento = DateTime.UtcNow; // Atualiza a data do pagamento concluído
            return true; // Pagamento concluído com sucesso
        }

        public void CancelarPagamento()
        {
            PagamentoConcluido = false;
            DataPagamento = DateTime.MinValue;
        }
    }
}
