using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record CadastrarUsuarioDto
    {
        [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
        public string Nome { get; init; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail deve ser válido.")]
        public string Email { get; init; }

        [Required(ErrorMessage = "O login do usuário é obrigatório.")]
        public string UsuarioLogin { get; init; }
    }
}
