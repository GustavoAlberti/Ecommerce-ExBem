using Application.QueryHandlers;
using Application.Querys;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using Moq;
using Shouldly;

namespace Test.UnitTests
{
    public class BuscarPedidoQueryHandlerTests
    {
        [Fact]
        public async Task BuscarPedido_DeveRetornarPedido_ComSucesso()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var produto = new Produto("Produto Teste", "PROD01", 100, 50);
            var itemPedido = new ItemPedido(produto, 2, 100);
            var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");
            var pedido = new Pedido(usuario.Id);
            typeof(Pedido).GetProperty(nameof(Pedido.Usuario)).SetValue(pedido, usuario);

            pedido.AdicionarItem(itemPedido);
            pedido.AlterarStatus(StatusPedido.Concluido);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            var handler = new BuscarPedidoQueryHandler(pedidoRepositoryMock.Object);

            var query = new BuscarPedidoQuery(pedido.CodigoPedido); 

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.CodigoPedido.ShouldBe(pedido.CodigoPedido);
            result.UsuarioNome.ShouldBe(usuario.Nome);
            result.Itens.Count.ShouldBe(1);
            result.ValorTotal.ShouldBe(200);
            result.Status.ShouldBe(StatusPedido.Concluido.ToString());
        }

        [Fact]
        public async Task BuscarPedido_DeveLancarKeyNotFoundException_SePedidoNaoForEncontrado()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();

            var query = new BuscarPedidoQuery("PEDIDO123");

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync((Pedido)null);

            var handler = new BuscarPedidoQueryHandler(pedidoRepositoryMock.Object);

            // Act & Assert
            await Should.ThrowAsync<KeyNotFoundException>(async () =>
            {
                await handler.Handle(query, CancellationToken.None);
            });
        }
    }
}
