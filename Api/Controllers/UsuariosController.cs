using Application.Commands;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CadastrarUsuario")]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastrarUsuarioDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CadastrarUsuarioCommand(
                request.Nome,
                request.UsuarioLogin,
                request.Email
            );

            var resultado = await _mediator.Send(command);

            if (resultado)
                return CreatedAtAction(nameof(CadastrarUsuario), new { usuarioLogin = request.UsuarioLogin }, request);
        
            return BadRequest(new { mensagem = "Erro ao cadastrar o usuário." });
            
        }
    }
}
