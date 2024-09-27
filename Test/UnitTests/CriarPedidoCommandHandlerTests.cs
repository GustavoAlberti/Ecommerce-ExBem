using Application.Commands;
using Application.Handlers;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Moq;
using Shouldly;

namespace Test.UnitTests
{
    public class CriarPedidoCommandHandlerTests
    {
        [Fact]
        public async Task CriarPedido_DeveRetornarPedidoCriado_ComSucesso()
        {
            // Arrange
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var descontoRepositoryMock = new Mock<IDescontoRepository>();

            var command = new CriarPedidoCommand("usuario123", new List<CriarItemPedidoCommand>
            {
                new CriarItemPedidoCommand("PROD01", 2)
            });

            var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");
            var produto = new Produto("Produto Teste", "PROD01", 100, 50);

            
            usuarioRepositoryMock.Setup(x => x.ObterPorLoginAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            produtoRepositoryMock.Setup(x => x.ObterPorCodigosAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(new List<Produto> { produto });

            var descontos = new List<Desconto>
            {
                new Desconto(1, 10, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)) 
            };

            descontoRepositoryMock.Setup(x => x.ObterDescontosValidosAsync(It.IsAny<IEnumerable<int>>()))
                                  .ReturnsAsync(descontos);

            pedidoRepositoryMock.Setup(x => x.AdicionarAsync(It.IsAny<Pedido>())).Returns(Task.CompletedTask);

            var handler = new CriarPedidoCommandHandler(
                pedidoRepositoryMock.Object,
                usuarioRepositoryMock.Object,
                produtoRepositoryMock.Object,
                descontoRepositoryMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.CodigoPedido.ShouldNotBeNullOrEmpty();
            result.UsuarioNome.ShouldBe("Usuário Teste");
            result.Itens.Count.ShouldBe(1);
            result.ValorTotal.ShouldBe(200);
        }

        [Fact]
        public async Task CriarPedido_DeveLancarExcecao_SeUsuarioNaoForEncontrado()
        {
            // Arrange
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var descontoRepositoryMock = new Mock<IDescontoRepository>();

            var command = new CriarPedidoCommand("usuarioInvalido", new List<CriarItemPedidoCommand>
            {
                new CriarItemPedidoCommand("PROD01", 2)
            });

            usuarioRepositoryMock.Setup(x => x.ObterPorLoginAsync(It.IsAny<string>()))
                                 .ReturnsAsync((Usuario)null);

            var handler = new CriarPedidoCommandHandler(
                pedidoRepositoryMock.Object,
                usuarioRepositoryMock.Object,
                produtoRepositoryMock.Object,
                descontoRepositoryMock.Object
            );

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task CriarPedido_DeveLancarExcecao_SeProdutoNaoForEncontrado()
        {
            // Arrange
            var usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var produtoRepositoryMock = new Mock<IProdutoRepository>();
            var descontoRepositoryMock = new Mock<IDescontoRepository>();

            var command = new CriarPedidoCommand("usuario123", new List<CriarItemPedidoCommand>
            {
                new CriarItemPedidoCommand("PROD01", 2)
            });

            var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");

            usuarioRepositoryMock.Setup(x => x.ObterPorLoginAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            produtoRepositoryMock.Setup(x => x.ObterPorCodigosAsync(It.IsAny<IEnumerable<string>>()))
                                 .ReturnsAsync(new List<Produto>());

            var handler = new CriarPedidoCommandHandler(
                pedidoRepositoryMock.Object,
                usuarioRepositoryMock.Object,
                produtoRepositoryMock.Object,
                descontoRepositoryMock.Object
            );

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}
