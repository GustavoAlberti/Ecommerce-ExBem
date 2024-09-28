using Application.Commands;
using Application.DTOs;
using Application.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CriarPedido")]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CriarPedidoCommand(
                request.UsuarioLogin,
                request.Itens.Select(i => new CriarItemPedidoCommand(i.CodigoProduto, i.Quantidade))
            );

            var pedidoCriado = await _mediator.Send(command);

            return CreatedAtAction(nameof(CriarPedido), new { codigoPedido = pedidoCriado.CodigoPedido }, pedidoCriado);
        }

        [HttpPut("CancelarPedido/{codigoPedido}")]
        public async Task<IActionResult> CancelarPedido(string codigoPedido)
        {
            var command = new CancelarPedidoCommand(codigoPedido);

            await _mediator.Send(command);
            return Ok(new { mensagem = "Pedido cancelado com sucesso" });
            
        }

        [HttpGet("BuscarPorCodigo/{codigoPedido}")]
        public async Task<IActionResult> BuscarPorCodigoPedido(string codigoPedido)
        {
            var query = new BuscarPedidoQuery(codigoPedido);
            
            var pedido = await _mediator.Send(query);
                return Ok(pedido);
        }

        [HttpPost("{codigoPedido}/separar")]
        public async Task<IActionResult> SepararPedido(string codigoPedido)
        {
            var command = new SepararPedidoCommand(codigoPedido);

            var resultado = await _mediator.Send(command);

            if (!resultado)
                return BadRequest(new { mensagem = "Erro ao separar o pedido. Verifique o estoque ou o estado do pedido." });
            
            return Ok(new { mensagem = "Pedido separado com sucesso!" });
        }


    }

}
