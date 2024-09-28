using Application.Commands;
using Application.Handlers;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.UnitTests
{
    public class CadastrarProdutoCommandHandlerTests
    {
        [Fact]
        public async Task CadastrarProduto_DeveCadastrarProdutoComSucesso()
        {
            // Arrange
            var produtoRepositoryMock = new Mock<IProdutoRepository>();

            var command = new CadastrarProdutoCommand("Produto Teste", "PROD01", 100.0m, 50);

            produtoRepositoryMock.Setup(repo => repo.ObterPorCodigosAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<Produto>());

            var handler = new CadastrarProdutoCommandHandler(produtoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldBeTrue();
            produtoRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
        }

        [Fact]
        public async Task CadastrarProduto_DeveLancarExcecao_SeProdutoJaExistir()
        {
            // Arrange
            var produtoRepositoryMock = new Mock<IProdutoRepository>();

            var command = new CadastrarProdutoCommand("Produto Teste", "PROD01", 100.0m, 50);

            var produtoExistente = new Produto("Produto Existente", "PROD01", 100.0m, 50);
            produtoRepositoryMock.Setup(repo => repo.ObterPorCodigosAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<Produto> { produtoExistente });

            var handler = new CadastrarProdutoCommandHandler(produtoRepositoryMock.Object);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            produtoRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Produto>()), Times.Never);
        }

    }
}
