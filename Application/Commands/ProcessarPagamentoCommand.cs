using Application.DTOs;
using Domain.Entities.Enum;
using MediatR;

namespace Application.Commands
{
    public class ProcessarPagamentoCommand : IRequest<PagamentoResponseDto>
    {
        public string CodigoPedido { get; }
        public TipoPagamento TipoPagamento { get; }
        public int? NumeroParcelas { get; }

        public ProcessarPagamentoCommand(string codigoPedido, TipoPagamento tipoPagamento, int? numeroParcelas = null)
        {
            CodigoPedido = codigoPedido;
            TipoPagamento = tipoPagamento;
            NumeroParcelas = numeroParcelas;
        }
    }

}
