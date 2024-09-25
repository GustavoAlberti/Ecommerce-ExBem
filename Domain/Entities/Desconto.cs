using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Desconto
    {
        public int Id { get; private set; }
        public int? ProdutoId { get; private set; } // Se o desconto é específico para um produto
        public decimal ValorDesconto { get; private set; } // Valor em reais ou percentual
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }

        private Desconto() { }

        // Construtor para inicializar um desconto
        public Desconto(int? produtoId, decimal valorDesconto, DateTime dataInicio, DateTime dataFim)
        {
            ProdutoId = produtoId;
            ValorDesconto = valorDesconto;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        // Método para verificar se o desconto é válido para a data fornecida
        public bool IsValido(DateTime dataPedido)
        {
            return DataInicio <= dataPedido && dataPedido <= DataFim;
        }
    }
}
