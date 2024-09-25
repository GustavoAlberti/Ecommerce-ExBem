using Application.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record PagarPedidoDto
    {
        [Required(ErrorMessage = "O código do pedido é obrigatório.")]
        public string CodigoPedido { get; init; }

        [Required(ErrorMessage = "O tipo de pagamento é obrigatório.")]
        public TipoPagamento TipoPagamento { get; init; } // Usando o enum de forma clara

        [Range(1, 12, ErrorMessage = "O número de parcelas deve estar entre 1 e 12.")]
        public int? NumeroParcelas { get; init; } // Aplicável apenas para Cartão de Crédito
    }

}
