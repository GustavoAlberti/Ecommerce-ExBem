using Application.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PagamentoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <remarks>
        /// Processa o pagamento de um pedido.
        /// 
        /// Tipos de Pagamento:
        /// 1 - Pix (Desconto de 5%)
        /// 2 - Cartão de Crédito (Deve ser informado o número de parcelas)
        /// 
        /// Caso o pagamento seja via Cartão de Crédito, o número de parcelas deve ser enviado no campo "numeroParcelas" com um valor entre 1 e 12.
        ///
        /// Caso seja Pix enviar null
        /// </remarks>
        [HttpPost("pagar")]
        public async Task<IActionResult> PagarPedido([FromBody] PagarPedidoDto pagamentoRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new ProcessarPagamentoCommand(
                pagamentoRequest.CodigoPedido,
                pagamentoRequest.TipoPagamento,
                pagamentoRequest.NumeroParcelas
            );

            var pagamentoResponse = await _mediator.Send(command);

            return pagamentoResponse.Status == "Cancelado" ? BadRequest(pagamentoResponse) : Ok(pagamentoResponse);
        }
    }

}
