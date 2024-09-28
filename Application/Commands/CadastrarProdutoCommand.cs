using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
