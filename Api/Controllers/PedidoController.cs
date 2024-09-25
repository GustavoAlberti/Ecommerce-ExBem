using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // POST: api/pedidos/criarPedido
        [HttpPost("CriarPedido")]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Chama o serviço para criar o pedido
                var pedidoCriado = await _pedidoService.CriarPedidoAsync(dto);

                // Retorna o pedido criado
                return CreatedAtAction(nameof(CriarPedido), new { codigoPedido = pedidoCriado.CodigoPedido }, pedidoCriado);
            }
            catch (Exception ex)
            {
                // Retorna erro em caso de exceção
                return StatusCode(500, new { mensagem = "Erro ao criar pedido", detalhes = ex.Message });
            }
        }

        [HttpGet("BuscarPorCodigo/{codigoPedido}")]
        public async Task<IActionResult> BuscarPorCodigoPedido(string codigoPedido)
        {
            try
            {
                var pedido = await _pedidoService.BuscarPorCodigoPedidoAsync(codigoPedido);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        // PUT: api/pedidos/CancelarPedido/{codigoPedido}
        [HttpPut("CancelarPedido/{codigoPedido}")]
        public async Task<IActionResult> CancelarPedido(string codigoPedido)
        {
            try
            {
                await _pedidoService.CancelarPedidoAsync(codigoPedido);
                return Ok(new { mensagem = "Pedido cancelado com sucesso" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }


    }

}
