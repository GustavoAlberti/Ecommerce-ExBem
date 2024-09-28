using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record CadastrarProdutoDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        public string Nome { get; init; }

        [Required(ErrorMessage = "O código do produto é obrigatório.")]
        public string CodigoProduto { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; init; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
        [DefaultValue("1")]
        public int QuantidadeEmEstoque { get; init; }
    }
}
