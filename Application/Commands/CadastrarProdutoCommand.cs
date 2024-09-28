using MediatR;

namespace Application.Commands
{
    public class CadastrarProdutoCommand : IRequest<bool>
    {
        public string Nome { get; }
        public string CodigoProduto { get; }
        public decimal Preco { get; }
        public int QuantidadeEmEstoque { get; }

        public CadastrarProdutoCommand(string nome, string codigoProduto, decimal preco, int quantidadeEmEstoque)
        {
            Nome = nome;
            CodigoProduto = codigoProduto;
            Preco = preco;
            QuantidadeEmEstoque = quantidadeEmEstoque;
        }
    }
}
