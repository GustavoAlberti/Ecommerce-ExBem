using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoResponseDto> CriarPedidoAsync(CriarPedidoDto dto);
        Task<PedidoResponseDto> BuscarPorCodigoPedidoAsync(string codigoPedido);
        Task CancelarPedidoAsync(string codigoPedido);
    }

}
