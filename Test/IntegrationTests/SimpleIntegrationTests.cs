using Domain.Entities;
using Domain.Entities.Enum;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace Test.IntegrationTests
{
    public class SimpleIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _output;  // Helper para capturar a saída de logs

        public SimpleIntegrationTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _output = output;
            _output.WriteLine("Factory e Client configurados.");
        }

        [Fact]
        public async Task TesteDeBancoDeDadosEmMemoria()
        {
            Console.WriteLine("Iniciando o teste...");

            using (var scope = _factory.Services.CreateScope())
            {
                _output.WriteLine("Iniciando o teste...");

                var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

                // Adiciona um pedido simples ao banco de dados em memória
                var pedido = new Pedido(1);
                pedido.AlterarStatus(StatusPedido.AguardandoPagamento);
                typeof(Pedido).GetProperty("CodigoPedido").SetValue(pedido, "TEST1234");

                _output.WriteLine("Adicionando pedido ao contexto...");
                context.Pedidos.Add(pedido);
                await context.SaveChangesAsync();
                _output.WriteLine("Pedido salvo.");

                // Verifica se o pedido foi salvo
                var savedPedido = await context.Pedidos.SingleOrDefaultAsync(p => p.CodigoPedido == "TEST1234");
                _output.WriteLine("Pedido encontrado? " + (savedPedido != null ? "Sim" : "Não"));

                Assert.NotNull(savedPedido);
                Assert.Equal("TEST1234", savedPedido.CodigoPedido);
            }

            _output.WriteLine("Teste concluído.");
        }
    }


}
