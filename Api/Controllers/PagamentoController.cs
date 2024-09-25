using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
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
        public async Task<IActionResult> PagarPedido([FromBody] PagarPedidoDto pagamentoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pagamentoResponse = await _pagamentoService.ProcessarPagamentoAsync(pagamentoDto);

            return pagamentoResponse.Status == "Cancelado" ? BadRequest(pagamentoResponse) : Ok(pagamentoResponse);
        }
    }
}
