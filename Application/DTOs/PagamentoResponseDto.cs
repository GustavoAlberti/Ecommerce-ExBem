using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record PagamentoResponseDto(
        string Mensagem,
        decimal ValorTotal,
        decimal? ValorComDesconto, 
        int? NumeroParcelas, 
        string TipoPagamento,
        string Status
    );


}
