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
    public class CancelarPedidoCommandHandlerTests
    {
        [Fact]
        public async Task CancelarPedido_DeveAtualizarStatusParaCancelado_ComSucesso()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoRepositoryMock = new Mock<IPagamentoRepository>();
            var notificacaoServiceMock = new Mock<INotificacaoService>();

            var command = new CancelarPedidoCommand("PEDIDO123");

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), true))
                                .ReturnsAsync(pedido);

            pedidoRepositoryMock.Setup(x => x.AtualizarAsync(It.IsAny<Pedido>()))
                                .Returns(Task.CompletedTask);

            notificacaoServiceMock.Setup(x => x.EnviarNotificacaoStatusPedidoAsync(It.IsAny<Pedido>(), It.IsAny<string>()))
                                  .Returns(Task.CompletedTask);

            var handler = new CancelarPedidoCommandHandler(pedidoRepositoryMock.Object, pagamentoRepositoryMock.Object, notificacaoServiceMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            pedido.Status.ShouldBe(StatusPedido.Cancelado);
            notificacaoServiceMock.Verify(x => x.EnviarNotificacaoStatusPedidoAsync(pedido, "Seu pedido foi cancelado e o pagamento estornado."), Times.Once);
        }

        [Fact]
        public async Task CancelarPedido_DeveLancarInvalidOperationException_SeStatusInvalido()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoRepositoryMock = new Mock<IPagamentoRepository>();
            var notificacaoServiceMock = new Mock<INotificacaoService>();

            var command = new CancelarPedidoCommand("PEDIDO123");

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.Concluido);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), true))
                                .ReturnsAsync(pedido);

            var handler = new CancelarPedidoCommandHandler(pedidoRepositoryMock.Object, pagamentoRepositoryMock.Object, notificacaoServiceMock.Object);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task CancelarPedido_DeveCancelarPagamento_SeStatusAguardandoEstoque()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoRepositoryMock = new Mock<IPagamentoRepository>();
            var notificacaoServiceMock = new Mock<INotificacaoService>();

            var command = new CancelarPedidoCommand("PEDIDO123");

            var pagamento = new Pagamento(1, TipoPagamento.CartaoDeCredito, 200);
            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.AguardandoEstoque);

            // Utilizando FieldInfo para definir o campo privado diretamente
            typeof(Pedido).GetField("<Pagamento>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                          ?.SetValue(pedido, pagamento);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), true))
                                .ReturnsAsync(pedido);

            pagamentoRepositoryMock.Setup(x => x.AtualizarPagamentoAsync(It.IsAny<Pagamento>()))
                                   .Returns(Task.CompletedTask);

            var handler = new CancelarPedidoCommandHandler(pedidoRepositoryMock.Object, pagamentoRepositoryMock.Object, notificacaoServiceMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            pedido.Status.ShouldBe(StatusPedido.Cancelado);
            pagamento.PagamentoConcluido.ShouldBeFalse();
            pagamentoRepositoryMock.Verify(x => x.AtualizarPagamentoAsync(pagamento), Times.Once);
        }

    }
}
