using Application.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProdutosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CadastrarProduto")]
        public async Task<IActionResult> CadastrarProduto([FromBody] CadastrarProdutoDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CadastrarProdutoCommand(
                request.Nome,
                request.CodigoProduto,
                request.Preco,
                request.QuantidadeEmEstoque
            );

            var resultado = await _mediator.Send(command);

            if (resultado)
                return CreatedAtAction(nameof(CadastrarProduto), new { codigoProduto = request.CodigoProduto }, request);
            
            return BadRequest(new { mensagem = "Erro ao cadastrar o produto." });
            
        }
    }
}
