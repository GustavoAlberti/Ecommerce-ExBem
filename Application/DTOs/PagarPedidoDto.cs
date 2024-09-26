using Domain.Entities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record PagarPedidoDto
    {
        [Required(ErrorMessage = "O código do pedido é obrigatório.")]
        public string CodigoPedido { get; init; }

        [Required(ErrorMessage = "O tipo de pagamento é obrigatório.")]
        public TipoPagamento TipoPagamento { get; init; }

        [Range(1, 12, ErrorMessage = "O número de parcelas deve estar entre 1 e 12.")]
        public int? NumeroParcelas { get; init; }
    }

}
