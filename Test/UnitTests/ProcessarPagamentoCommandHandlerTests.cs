using Application.Commands;
using Application.Handlers;
using Application.Interfaces.PagamentoStrategy;
using Domain.Entities;
using Domain.Entities.Enum;
using Domain.Interfaces.Repositories;
using Moq;
using Shouldly;

namespace Test.UnitTests
{
    public class ProcessarPagamentoCommandHandlerTests
    {
        [Fact]
        public async Task ProcessarPagamento_Pix_DeveConcluirComDesconto()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoStrategyMock = new Mock<IPagamentoStrategy>();
            var pagamentoStrategiesMock = new Dictionary<TipoPagamento, IPagamentoStrategy>
            {
                { TipoPagamento.Pix, pagamentoStrategyMock.Object }
            };

            var command = new ProcessarPagamentoCommand("PEDIDO123", TipoPagamento.Pix);

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            pagamentoStrategyMock.Setup(x => x.ProcessarPagamentoAsync(It.IsAny<Pedido>(), null))
                                 .ReturnsAsync(true); // Simula sucesso no pagamento

            var handler = new ProcessarPagamentoCommandHandler(pagamentoStrategiesMock, pedidoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe("Pagamento Concluído");
            result.Mensagem.ShouldBe("Pagamento realizado com sucesso.");
            result.TipoPagamento.ShouldBe("Pix");
        }


        [Fact]
        public async Task ProcessarPagamento_Cartao_DeveConcluirComNumeroParcelas()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoStrategyMock = new Mock<IPagamentoStrategy>();
            var pagamentoStrategiesMock = new Dictionary<TipoPagamento, IPagamentoStrategy>
            {
                { TipoPagamento.CartaoDeCredito, pagamentoStrategyMock.Object }
            };

            var command = new ProcessarPagamentoCommand("PEDIDO123", TipoPagamento.CartaoDeCredito, numeroParcelas: 3);

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            var pagamento = new Pagamento(pedido.Id, TipoPagamento.CartaoDeCredito, 300, 3);

            typeof(Pedido).GetField("<Pagamento>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                          ?.SetValue(pedido, pagamento);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            pagamentoStrategyMock.Setup(x => x.ProcessarPagamentoAsync(It.IsAny<Pedido>(), 3))
                                 .ReturnsAsync(true);

            var handler = new ProcessarPagamentoCommandHandler(pagamentoStrategiesMock, pedidoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe("Pagamento Concluído");
            result.Mensagem.ShouldBe("Pagamento realizado com sucesso.");
            result.TipoPagamento.ShouldBe("CartaoDeCredito");
            result.NumeroParcelas.ShouldBe(3);
        }

        [Fact]
        public async Task ProcessarPagamento_DeveRetornarErroSePedidoJaConcluido()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoStrategiesMock = new Dictionary<TipoPagamento, IPagamentoStrategy>();

            var command = new ProcessarPagamentoCommand("PEDIDO123", TipoPagamento.Pix);

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.PagamentoConcluido); // Pedido já foi pago

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            var handler = new ProcessarPagamentoCommandHandler(pagamentoStrategiesMock, pedidoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe("Pagamento Concluído");
            result.Mensagem.ShouldBe("O pagamento já foi realizado.");
        }

        [Fact]
        public async Task ProcessarPagamento_DeveRetornarFalhaSeTipoPagamentoInvalido()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoStrategiesMock = new Dictionary<TipoPagamento, IPagamentoStrategy>();

            var command = new ProcessarPagamentoCommand("PEDIDO123", (TipoPagamento)99);

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            var handler = new ProcessarPagamentoCommandHandler(pagamentoStrategiesMock, pedidoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe("Falha");
            result.Mensagem.ShouldBe("Tipo de pagamento inválido.");
        }

        [Fact]
        public async Task ProcessarPagamento_DeveCancelarPedidoSePagamentoFalhar()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoStrategyMock = new Mock<IPagamentoStrategy>();

            var pagamentoStrategiesMock = new Dictionary<TipoPagamento, IPagamentoStrategy>
            {
                { TipoPagamento.Pix, pagamentoStrategyMock.Object }
            };

            var command = new ProcessarPagamentoCommand("PEDIDO123", TipoPagamento.Pix);

            var pedido = new Pedido(1);
            pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync(pedido);

            // Simulando falha no pagamento
            pagamentoStrategyMock.Setup(x => x.ProcessarPagamentoAsync(It.IsAny<Pedido>(), null))
                                 .ReturnsAsync(false);

            var handler = new ProcessarPagamentoCommandHandler(pagamentoStrategiesMock, pedidoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe("Cancelado");
            result.Mensagem.ShouldBe("Falha no processamento do pagamento.");
        }

        [Fact]
        public async Task ProcessarPagamento_DeveRetornarFalhaSePedidoNaoEncontrado()
        {
            // Arrange
            var pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var pagamentoStrategiesMock = new Dictionary<TipoPagamento, IPagamentoStrategy>();

            var command = new ProcessarPagamentoCommand("PEDIDO123", TipoPagamento.Pix);

            pedidoRepositoryMock.Setup(x => x.ObterPorCodigoPedidoAsync(It.IsAny<string>(), false))
                                .ReturnsAsync((Pedido)null); 

            var handler = new ProcessarPagamentoCommandHandler(pagamentoStrategiesMock, pedidoRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.ShouldBe("Falha");
            result.Mensagem.ShouldBe("Pedido não encontrado.");
        }



    }

}
