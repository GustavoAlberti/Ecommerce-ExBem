using Application.Commands;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Handlers
{
    public class CadastrarProdutoCommandHandler : IRequestHandler<CadastrarProdutoCommand, bool>
    {
        private readonly IProdutoRepository _produtoRepository;

        public CadastrarProdutoCommandHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<bool> Handle(CadastrarProdutoCommand command, CancellationToken cancellationToken)
        {
            var produtosExistentes = await _produtoRepository.ObterPorCodigosAsync(new[] { command.CodigoProduto });

            if (produtosExistentes.Any(p => p.CodigoProduto == command.CodigoProduto))
            {
                throw new InvalidOperationException("Já existe um produto com o mesmo código.");
            }

            var produto = new Produto(command.Nome, command.CodigoProduto, command.Preco, command.QuantidadeEmEstoque);
            await _produtoRepository.AdicionarAsync(produto);

            return true;
        }
    }
}
