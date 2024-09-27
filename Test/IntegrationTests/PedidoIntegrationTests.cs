using Application.Commands;
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Enum;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using System.Text;
using Xunit;

namespace Test.IntegrationTests
{
    public class PedidoIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PedidoIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Configuração inicial de dados
            SetupInitialData().GetAwaiter().GetResult();
        }

        [Fact]
        public async Task CriarPedido_DeveSalvarERecuperarDoBancoDeDados()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

                var pedidoRepository = new PedidoRepository(context);

                var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");
                context.Usuarios.Add(usuario);

                var produto = new Produto("Produto Teste", "PROD01", 100, 50);
                context.Produtos.Add(produto);

                await context.SaveChangesAsync();

                // Act: Criar um novo pedido
                var pedido = new Pedido(usuario.Id);
                pedido.AdicionarItem(new ItemPedido(produto, 2, produto.Preco));
                pedido.AlterarStatus(StatusPedido.AguardandoPagamento);

                await pedidoRepository.AdicionarAsync(pedido);

                // Assert:
                var pedidoSalvo = await pedidoRepository.ObterPorCodigoPedidoAsync(pedido.CodigoPedido);
                Assert.NotNull(pedidoSalvo);
                Assert.Equal(pedido.CodigoPedido, pedidoSalvo.CodigoPedido);
                Assert.Equal(2, pedidoSalvo.Itens.First().Quantidade);
                Assert.Equal(200, pedidoSalvo.ValorTotal);
                Assert.Equal(StatusPedido.AguardandoPagamento, pedidoSalvo.Status);
            }
        }

        [Fact]
        public async Task CriarPedido_DeveRetornarPedidoCriado_ComSucesso_ViaApi()
        {
            // Arrange: Cria o comando para a requisição
            var command = new CriarPedidoCommand("usuario123", new List<CriarItemPedidoCommand>
            {
                new CriarItemPedidoCommand("PROD01", 2)
            });

            var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

            // Act: Envia a requisição POST para a API
            var response = await _client.PostAsync("/api/Pedidos/CriarPedido", content);

            // Assert: Verifica se o status de retorno é 200 OK
            response.EnsureSuccessStatusCode();

            // Verifica o conteúdo da resposta
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PedidoResponseDto>(jsonResponse);

            result.ShouldNotBeNull();
            result.CodigoPedido.ShouldNotBeNullOrEmpty();
            result.UsuarioNome.ShouldBe("Usuário Teste");
            result.Itens.Count.ShouldBe(1);
            result.ValorTotal.ShouldBe(200);
        }


        private async Task SetupInitialData()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

                if (!context.Usuarios.Any())
                {
                    // Adiciona o usuário e o produto ao banco de dados em memória
                    var usuario = new Usuario("Usuário Teste", "teste@email.com", "usuario123");
                    var produto = new Produto("Produto Teste", "PROD01", 100, 50);

                    context.Usuarios.Add(usuario);
                    context.Produtos.Add(produto);

                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
