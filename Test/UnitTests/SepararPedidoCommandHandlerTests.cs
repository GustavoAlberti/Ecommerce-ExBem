using Application.Commands;
using Application.Handlers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using Moq;
using Shouldly;

namespace Test.UnitTests
{
    public class SepararPedidoCommandHandlerTests
    {
        [Fact]
        public async Task SepararPedido_DeveAtualizarStatusParaConcluido_ComSucesso()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var notificacaoServiceMock = new Mock<INotificacaoService>();
            var command = new SepararPedidoCommand("PEDIDO123");

            var produto = new Produto("Produto Teste", "PROD001", 50, 10);
            var itemPedido = new ItemPedido(produto, 2, 50);
            var pedido = new Pedido(1);
            pedido.AdicionarItem(itemPedido);
            pedido.AlterarStatus(StatusPedido.PagamentoConcluido);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            var handler = new SepararPedidoCommandHandler(pedidoRepositoryMock.Object, notificacaoServiceMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            pedido.Status.ShouldBe(StatusPedido.Concluido);
        }

        [Fact]
        public async Task SepararPedido_DeveRetornarErroSeEstoqueInsuficiente()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var notificacaoServiceMock = new Mock<INotificacaoService>();
            var command = new SepararPedidoCommand("PEDIDO123");

            var produto = new Produto("Produto Teste", "PROD001", 50, 1);
            var itemPedido = new ItemPedido(produto, 2, 50);
            var pedido = new Pedido(1);
            pedido.AdicionarItem(itemPedido);
            pedido.AlterarStatus(StatusPedido.PagamentoConcluido);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            notificacaoServiceMock.Setup(x => x.EnviarNotificacaoEstoqueInsuficienteAsync(It.IsAny<Pedido>()))
                                  .Returns(Task.CompletedTask);

            var handler = new SepararPedidoCommandHandler(pedidoRepositoryMock.Object, notificacaoServiceMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            pedido.Status.ShouldBe(StatusPedido.AguardandoEstoque); 
            notificacaoServiceMock.Verify(x => x.EnviarNotificacaoEstoqueInsuficienteAsync(It.IsAny<Pedido>()), Times.Once);
        }

    }
}
