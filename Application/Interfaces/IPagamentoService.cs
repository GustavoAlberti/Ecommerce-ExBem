using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<PagamentoResponseDto> ProcessarPagamentoAsync(PagarPedidoDto pagamento);

    }
}